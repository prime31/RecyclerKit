using UnityEngine;
using System.Collections;
using System.Collections.Generic;



public class TrashMan : MonoBehaviour
{
	public static TrashMan instance { get; private set; }

	public List<TrashManRecycleBin> prefabPoolCollection;

	/// <summary>
	/// uses the GameObject instanceId as its key for fast look-ups
	/// </summary>
	private Dictionary<int,TrashManRecycleBin> _instanceIdToRecycleBin = new Dictionary<int,TrashManRecycleBin>();

	/// <summary>
	/// uses the pool name to find the GameObject instanceId
	/// </summary>
	private Dictionary<string,int> _poolNameToInstanceId = new Dictionary<string,int>();



	#region MonoBehaviour

	private void Awake()
	{
		if( instance != null )
		{
			Destroy( gameObject );
		}
		else
		{
			instance = this;
			initializePrefabPools();
		}

		StartCoroutine( cullExcessObjects() );
	}


	private void OnLevelWasLoaded()
	{
		// TODO: perhaps make this configurable per pool then add DontDestroyOnLoad. Currently this does nothing.
		_instanceIdToRecycleBin.Clear();
	}


	private void OnApplicationQuit()
	{
		instance = null;
	}
	
	#endregion


	#region Private/Public

	/// <summary>
	/// coroutine that runs every couple seconds and removes any objects created over the recycle bins limit
	/// </summary>
	/// <returns>The excess objects.</returns>
	private IEnumerator cullExcessObjects()
	{
		var waiter = new WaitForSeconds( 2f );

		while( true )
		{
			for( var i = 0; i < prefabPoolCollection.Count; i++ )
				prefabPoolCollection[i].cullExcessObjects();

			yield return waiter;
		}
	}


	/// <summary>
	/// populats the lookup dictionaries
	/// </summary>
	private void initializePrefabPools()
	{
		if( prefabPoolCollection == null )
			return;

		foreach( var prefabPool in prefabPoolCollection )
		{
			if( prefabPool == null || prefabPool.prefab == null )
				continue;

			prefabPool.initialize();
			_instanceIdToRecycleBin.Add( prefabPool.prefab.GetInstanceID(), prefabPool );
			_poolNameToInstanceId.Add( prefabPool.binName, prefabPool.prefab.GetInstanceID() );
		}
	}


	/// <summary>
	/// pulls an object out of the recycle bin
	/// </summary>
	public static GameObject spawn( string recycleBinName )
	{
		int instanceId = -1;
		if( instance._poolNameToInstanceId.TryGetValue( recycleBinName, out instanceId ) )
			return instance._instanceIdToRecycleBin[instanceId].spawn();

		return null;
	}


	/// <summary>
	/// 
	/// </summary>
	public static GameObject spawn( GameObject go )
	{
		var newGo = spawn( go.GetInstanceID() );
		if( newGo == null )
		{
			Debug.LogWarning( "attempted to spawn go (" + go.name + ") but there is no pool setup for it. Falling back to Instantiate" );
			newGo = GameObject.Instantiate( go ) as GameObject;
			newGo.transform.parent = instance.transform;
		}

		return newGo;
	}


	/// <summary>
	/// 
	/// </summary>
	private static GameObject spawn( int gameObjectInstanceId )
	{
		if( instance._instanceIdToRecycleBin.ContainsKey( gameObjectInstanceId ) )
		{
			var newGo = instance._instanceIdToRecycleBin[gameObjectInstanceId].spawn();
			newGo.transform.parent = null;
			return newGo;
		}

		return null;
	}


	/// <summary>
	/// 
	/// </summary>
	public static void despawn( GameObject go )
	{	
		if( go == null )
			return;

		if( !instance._instanceIdToRecycleBin.ContainsKey( go.GetInstanceID() ) )
			Destroy( go );
		else
			instance._instanceIdToRecycleBin[go.GetInstanceID()].despawn( go );
	}

	#endregion

}
