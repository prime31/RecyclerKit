using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;


[System.Serializable]
public sealed class TrashManRecycleBin
{
	/// <summary>
	/// Fired when the GameObject that was just spawned
	/// </summary>
	public event Action<GameObject> onSpawnedEvent;

	/// <summary>
	/// Fired when the GameObject that was just despawned
	/// </summary>
	public event Action<GameObject> onDespawnedEvent;


	/// <summary>
	/// The prefab or GameObject in the scene managed by this class.
	/// </summary>
	public GameObject prefab;

	/// <summary>
	/// total number of instances to create at start
	/// </summary>
	public int instancesToPreallocate = 5;

	/// <summary>
	/// total number of instances to allocate if one is requested when the bin is empty
	/// </summary>
	public int instancesToAllocateIfEmpty = 1;

	/// <summary>
	/// if true, the recycle bin will not create any more instances when hardLimit is reached and will instead return null for any spanws
	/// </summary>
	public bool imposeHardLimit = false;

	/// <summary>
	/// if imposeHardLimit is true, this will be the maximum number of instances to create
	/// </summary>
	public int hardLimit = 5;

	/// <summary>
	/// if true, any excess instances will be culled at regular intervals
	/// </summary>
	public bool cullExcessPrefabs = false;

	/// <summary>
	/// total instances to keep in the pool. All excess will be culled if cullExcessPrefabs is true
	/// </summary>
	public int instancesToMaintainInPool = 5;

	/// <summary>
	/// how often in seconds should culling occur
	/// </summary>
	public float cullInterval = 10f;


	/// <summary>
	/// stores all of our GameObjects
	/// </summary>
	private Stack<GameObject> _gameObjectPool;

	/// <summary>
	/// last time culling happened
	/// </summary>
	private float _timeOfLastCull = float.MinValue;

	/// <summary>
	/// keeps track of the total number of instances spawned
	/// </summary>
	private int _spawnedInstanceCount = 0;


	#region Private

	/// <summary>
	/// allocates
	/// </summary>
	/// <param name="count">Count.</param>
	private void allocateGameObjects( int count )
	{
		if( imposeHardLimit && _gameObjectPool.Count + count > hardLimit )
			count = hardLimit - _gameObjectPool.Count;

		for( int n = 0; n < count; n++ )
		{
			GameObject go = GameObject.Instantiate( prefab.gameObject ) as GameObject;
			go.name = prefab.name;
			go.transform.parent = TrashMan.instance.transform;
			go.SetActive( false );
			_gameObjectPool.Push( go );
		}
	}


	/// <summary>
	/// pops an object off the stack. Returns null if we hit the hardLimit.
	/// </summary>
	private GameObject pop()
	{
		if( imposeHardLimit && _spawnedInstanceCount >= hardLimit )
			return null;

		if( _gameObjectPool.Count > 0 )
		{
			_spawnedInstanceCount++;
			return _gameObjectPool.Pop();
		}

		allocateGameObjects( instancesToAllocateIfEmpty );
		return pop();
	}

	#endregion


	#region Public

	/// <summary>
	/// preps the Stack and does preallocation
	/// </summary>
	public void initialize()
	{
		//prefab.prefabPoolName = prefab.gameObject.name;
		_gameObjectPool = new Stack<GameObject>( instancesToPreallocate );
		allocateGameObjects( instancesToPreallocate );
	}


	/// <summary>
	/// culls any excess objects if necessary
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
	/// fetches a new instance from the recycle bin. Returns null if we reached the hardLimit.
	/// </summary>
	public GameObject spawn()
	{
		var go = pop();

		if( go != null )
		{
			if( onSpawnedEvent != null )
				onSpawnedEvent( go );
		}

		return go;
	}


	/// <summary>
	/// returns an instance to the recycle bin
	/// </summary>
	/// <param name="go">Go.</param>
	public void despawn( GameObject go )
	{
		go.SetActive( false );

		_spawnedInstanceCount--;
		_gameObjectPool.Push( go );

		if( onDespawnedEvent != null )
			onDespawnedEvent( go );
	}

	#endregion

}
