using UnityEngine;
using System.Collections;
using System.Collections.Generic;


[System.Serializable]
public class TrashManRecycleBin
{
	/// <summary>
	/// The prefab managed by this class.
	/// </summary>
	public GameObject prefab;

	/// <summary>
	/// the name of the pool. Used for spawning an object by name instead of having to use a GO reference
	/// </summary>
	public string binName;

	/// <summary>
	/// The number of the prefab to pre-allocate.
	/// </summary>
	public int instancesToPreallocate = 5;

	/// <summary>
	/// The number of this prefab to allocate when requesting from an empty pool.
	/// </summary>
	public int instancesToAllocateIfEmpty = 1;

	/// <summary>
	/// True if no instances beyond the limit should be created.
	/// </summary>
	public bool imposeHardLimit = false;

	/// <summary>
	/// The limit for hard limited pools.
	/// </summary>
	public int hardLimit = 5;

	/// <summary>
	/// True if excess prefabs should be culled.
	/// </summary>
	public bool cullExcessPrefabs = false;

	/// <summary>
	/// The maximum number of the prefab to maintain in the pool.
	/// </summary>
	public int instancesToMaintainInPool = 5;

	/// <summary>
	/// The frequency at which to cull excess prefabs.
	/// </summary>
	public float cullInterval = 10f;

	/// <summary>
	/// The pool of game objects
	/// </summary>
	private Stack<GameObject> _gameObjectPool;

	/// <summary>
	/// The time of the last cull.
	/// </summary>
	private float _timeOfLastCull = float.MinValue;

	/// <summary>
	/// How many instances have been requested?
	/// </summary>
	private int _spawnedObjectCount = 0;


	#region Methods

	public void initialize()
	{
		//prefab.prefabPoolName = prefab.gameObject.name;
		_gameObjectPool = new Stack<GameObject>( instancesToPreallocate );
		allocateGameObjects( instancesToPreallocate );
	}


	/// <summary>
	/// Adds the specified count of prefab instances to the pool.
	/// </summary>
	private void allocateGameObjects( int count )
	{
		if( imposeHardLimit && _gameObjectPool.Count + count > hardLimit )
			count = hardLimit - _gameObjectPool.Count;
		
		for( int n = 0; n < count; n++ )
		{
			GameObject go = GameObject.Instantiate( prefab.gameObject ) as GameObject;
			go.name = go.name + n.ToString();
			go.transform.parent = TrashMan.instance.transform;
			go.SetActive( false );
			_gameObjectPool.Push( go );
		}
	}


	/// <summary>
	/// Get a pooled item if one is available, or if legal create a new instance.
	/// </summary>
	public GameObject pop()
	{
		if( imposeHardLimit && _spawnedObjectCount >= hardLimit )
			return null;
		
		if( _gameObjectPool.Count > 0 )
		{
			_spawnedObjectCount++;
			return _gameObjectPool.Pop();
		}
		
		allocateGameObjects( instancesToAllocateIfEmpty );
		return pop();
	}


	/// <summary>
	/// Return an item to the pool.
	/// </summary>
	public void push( GameObject go )
	{
		if( imposeHardLimit && _gameObjectPool.Count >= hardLimit )
			return;
		
		_spawnedObjectCount = (int)Mathf.Max( _spawnedObjectCount - 1, 0 );
		_gameObjectPool.Push( go );
	}


	/// <summary>
	/// Poll for culling.
	/// </summary>
	public void cullExcessObjects()
	{
		if( !cullExcessPrefabs || _gameObjectPool.Count <= instancesToMaintainInPool )
			return;
		
		if( Time.time > _timeOfLastCull + cullInterval )
		{
			_timeOfLastCull = Time.time;
			for( int n = instancesToMaintainInPool; n <= _gameObjectPool.Count; n++ )
				GameObject.Destroy( _gameObjectPool.Pop() );
		}
	}


	/// <summary>
	/// Spawn an object from the pool.
	/// </summary>
	public GameObject spawn()
	{
		GameObject go = pop();
		
		if( go != null )
		{
			go.SetActive( true );
			// TODO: associate a method or event with the spawn action
		}
		
		return go;
	}


	/// <summary>
	/// Despawn an object back to the pool.
	/// </summary>
	public void despawn( GameObject go )
	{
		go.SetActive( false );
		// TODO: associate a method or event with the despawn action

		push( go );
	}

	#endregion

}
