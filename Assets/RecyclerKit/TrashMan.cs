using UnityEngine;
using System.Collections;
using System.Collections.Generic;



public partial class TrashMan : MonoBehaviour
{
	/// <summary>
	/// access to the singleton
	/// </summary>
	public static TrashMan instance;

	/// <summary>
	/// stores the recycle bins and is used to populate the lookup Dictionaries at startup
	/// </summary>
	[HideInInspector]
	public List<TrashManRecycleBin> recycleBinCollection;

	/// <summary>
	/// this is how often in seconds TrashMan should cull excess objects. Setting this to 0 or a negative number will
	/// fully turn off automatic culling. You can then use the TrashManRecycleBin.cullExcessObjects method manually if
	/// you would still like to do any culling.
	/// </summary>
	public float cullExcessObjectsInterval = 10f;

	/// <summary>
	/// if true, DontDestroyOnLoad will be called on the TrashMan
	/// </summary>
	public bool persistBetweenScenes = false;

	/// <summary>
	/// uses the GameObject instanceId as its key for fast look-ups
	/// </summary>
	private Dictionary<int,TrashManRecycleBin> _instanceIdToRecycleBin = new Dictionary<int,TrashManRecycleBin>();

	/// <summary>
	/// uses the pool name to find the GameObject instanceId
	/// </summary>
	private Dictionary<string,int> _poolNameToInstanceId = new Dictionary<string,int>();

	[HideInInspector]
	public new Transform transform;


	#region MonoBehaviour

	private void Awake()
	{
		if( instance != null )
		{
			Destroy( gameObject );
		}
		else
		{
			transform = gameObject.transform;
			instance = this;
			initializePrefabPools();

			if( persistBetweenScenes )
				DontDestroyOnLoad( gameObject );
		}

		// only cull if we have an interval greater than 0
		if( cullExcessObjectsInterval > 0 )
			StartCoroutine( cullExcessObjects() );
	}


	private void OnLevelWasLoaded()
	{
		for( var i = recycleBinCollection.Count - 1; i >= 0; i-- )
		{
			if( !recycleBinCollection[i].persistBetweenScenes )
				removeRecycleBin( recycleBinCollection[i] );
		}
	}


	private void OnApplicationQuit()
	{
		instance = null;
	}

	#endregion


	#region Private

	/// <summary>
	/// coroutine that runs every couple seconds and removes any objects created over the recycle bins limit
	/// </summary>
	/// <returns>The excess objects.</returns>
	private IEnumerator cullExcessObjects()
	{
		var waiter = new WaitForSeconds( cullExcessObjectsInterval );

		while( true )
		{
			for( var i = 0; i < recycleBinCollection.Count; i++ )
				recycleBinCollection[i].cullExcessObjects();

			yield return waiter;
		}
	}


	/// <summary>
	/// populats the lookup dictionaries
	/// </summary>
	private void initializePrefabPools()
	{
		if( recycleBinCollection == null )
			return;

		foreach( var recycleBin in recycleBinCollection )
		{
			if( recycleBin == null || recycleBin.prefab == null )
				continue;

			recycleBin.initialize();
			_instanceIdToRecycleBin.Add( recycleBin.prefab.GetInstanceID(), recycleBin );
			_poolNameToInstanceId.Add( recycleBin.prefab.name, recycleBin.prefab.GetInstanceID() );
		}
	}


	/// <summary>
	/// internal method that actually does the work of grabbing the item from the bin and returning it
	/// </summary>
	/// <param name="gameObjectInstanceId">Game object instance identifier.</param>
	private static GameObject spawn( int gameObjectInstanceId, Vector3 position, Quaternion rotation )
	{
		if( instance._instanceIdToRecycleBin.ContainsKey( gameObjectInstanceId ) )
		{
			var newGo = instance._instanceIdToRecycleBin[gameObjectInstanceId].spawn();

			if( newGo != null )
			{
				var newTransform = newGo.transform;

#if UNITY_4_6 || UNITY_5_0
                if (newTransform as RectTransform)
                    newTransform.SetParent(null, false);
                else
#endif
				    newTransform.parent = null;

				newTransform.position = position;
				newTransform.rotation = rotation;

				newGo.SetActive( true );
			}

			return newGo;
		}

		return null;
	}


	/// <summary>
	/// internal coroutine for despawning after a delay
	/// </summary>
	/// <returns>The despawn after delay.</returns>
	/// <param name="go">Go.</param>
	/// <param name="delayInSeconds">Delay in seconds.</param>
	private IEnumerator internalDespawnAfterDelay( GameObject go, float delayInSeconds )
	{
		yield return new WaitForSeconds( delayInSeconds );
		despawn( go );
	}

	#endregion


	#region Public

	/// <summary>
	/// tells TrashMan to start managing the recycle bin at runtime
	/// </summary>
	/// <param name="recycleBin">Recycle bin.</param>
	public static void manageRecycleBin( TrashManRecycleBin recycleBin )
	{
		// make sure we can safely add the bin!
		if( instance._poolNameToInstanceId.ContainsKey( recycleBin.prefab.name ) )
		{
			Debug.LogError( "Cannot manage the recycle bin because there is already a GameObject with the name (" + recycleBin.prefab.name + ") being managed" );
			return;
		}

		instance.recycleBinCollection.Add( recycleBin );
		recycleBin.initialize();
		instance._instanceIdToRecycleBin.Add( recycleBin.prefab.GetInstanceID(), recycleBin );
		instance._poolNameToInstanceId.Add( recycleBin.prefab.name, recycleBin.prefab.GetInstanceID() );
	}


	/// <summary>
	/// stops managing the recycle bin optionally destroying all managed objects
	/// </summary>
	/// <param name="recycleBin">Recycle bin.</param>
	/// <param name="shouldDestroyAllManagedObjects">If set to <c>true</c> should destroy all managed objects.</param>
	public static void removeRecycleBin( TrashManRecycleBin recycleBin, bool shouldDestroyAllManagedObjects = true )
	{
		var recycleBinName = recycleBin.prefab.name;

		// make sure we are managing the bin first
		if( instance._poolNameToInstanceId.ContainsKey( recycleBinName ) )
		{
			instance._poolNameToInstanceId.Remove( recycleBinName );
			instance._instanceIdToRecycleBin.Remove( recycleBin.prefab.GetInstanceID() );
			instance.recycleBinCollection.Remove( recycleBin );
			recycleBin.clearBin( shouldDestroyAllManagedObjects );
		}
	}


	/// <summary>
	/// pulls an object out of the recycle bin
	/// </summary>
	/// <param name="go">Go.</param>
	public static GameObject spawn( GameObject go, Vector3 position = default( Vector3 ), Quaternion rotation = default( Quaternion ) )
	{
		if( instance._instanceIdToRecycleBin.ContainsKey( go.GetInstanceID() ) )
		{
			return spawn( go.GetInstanceID(), position, rotation );
		}
		else
		{
			Debug.LogWarning( "attempted to spawn go (" + go.name + ") but there is no recycle bin setup for it. Falling back to Instantiate" );
			var newGo = GameObject.Instantiate( go, position, rotation ) as GameObject;

#if UNITY_4_6 || UNITY_5_0
            if (newGo.transform as RectTransform != null)
                newGo.transform.SetParent(null, false);
            else
#endif
			    newGo.transform.parent = null;

			return newGo;
		}
	}


	/// <summary>
	/// pulls an object out of the recycle bin using the bin's name
	/// </summary>
	public static GameObject spawn( string gameObjectName, Vector3 position = default( Vector3 ), Quaternion rotation = default( Quaternion ) )
	{
		int instanceId = -1;
		if( instance._poolNameToInstanceId.TryGetValue( gameObjectName, out instanceId ) )
		{
			return spawn( instanceId, position, rotation );
		}
		else
		{
			Debug.LogError( "attempted to spawn a GameObject from recycle bin (" + gameObjectName + ") but there is no recycle bin setup for it" );
			return null;
		}
	}


	/// <summary>
	/// sticks the GameObject back into its recycle bin. If the GameObject has no bin it is destroyed.
	/// </summary>
	/// <param name="go">Go.</param>
	public static void despawn( GameObject go )
	{
		if( go == null )
			return;

		var goName = go.name;
		if( !instance._poolNameToInstanceId.ContainsKey( goName ) )
		{
			Destroy( go );
		}
		else
		{
			instance._instanceIdToRecycleBin[instance._poolNameToInstanceId[goName]].despawn( go );

#if UNITY_4_6 || UNITY_5_0
            if (go.transform as RectTransform != null)
                go.transform.SetParent(instance.transform, false);
            else
#endif
                go.transform.parent = instance.transform;
		}
	}


	/// <summary>
	/// sticks the GameObject back into it's recycle bin after a delay. If the GameObject has no bin it is destroyed.
	/// </summary>
	/// <param name="go">Go.</param>
	public static void despawnAfterDelay( GameObject go, float delayInSeconds )
	{
		if( go == null )
			return;

		instance.StartCoroutine( instance.internalDespawnAfterDelay( go, delayInSeconds ) );
	}


	/// <summary>
	/// gets the recycle bin for the given GameObject name. Returns null if none exists.
	/// </summary>
	public static TrashManRecycleBin recycleBinForGameObjectName( string gameObjectName )
	{
		if( instance._poolNameToInstanceId.ContainsKey( gameObjectName ) )
		{
			var instanceId = instance._poolNameToInstanceId[gameObjectName];
			return instance._instanceIdToRecycleBin[instanceId];
		}
		return null;
	}


	/// <summary>
	/// gets the recycle bin for the given GameObject. Returns null if none exists.
	/// </summary>
	/// <returns>The bin for game object.</returns>
	/// <param name="go">Go.</param>
	public static TrashManRecycleBin recycleBinForGameObject( GameObject go )
	{
		if( instance._instanceIdToRecycleBin.ContainsKey( go.GetInstanceID() ) )
			return instance._instanceIdToRecycleBin[go.GetInstanceID()];
		return null;
	}


	#endregion

}
