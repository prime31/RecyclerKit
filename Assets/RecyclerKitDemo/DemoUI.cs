using UnityEngine;
using System.Collections;



public class DemoUI : MonoBehaviour
{
	// instance variables with links to the prefabs that we will be spawning
	public GameObject cubePrefab;
	public GameObject spherePrefab;
	public GameObject capsulePrefab;

	private bool _didCreateCapsuleRecycleBin;


	void Start()
	{
		// if you plan on listening to the spawn/despawn events, Start is a good time to add your listeners.
		TrashMan.recycleBinForGameObject( cubePrefab ).onSpawnedEvent += go => Debug.Log( "spawned object: " + go );
		TrashMan.recycleBinForGameObject( cubePrefab ).onDespawnedEvent += go => Debug.Log( "DEspawned object: " + go );
	}


	void OnGUI()
	{
		if( GUILayout.Button( "Spawn Cube" ) )
		{
			var newObj = TrashMan.spawn( cubePrefab, Random.onUnitSphere * 5f, Random.rotation );
			TrashMan.despawnAfterDelay( newObj, Random.Range( 1f, 2f ) );
		}


		if( GUILayout.Button( "Spawn Sphere" ) )
		{
			var newObj = TrashMan.spawn( spherePrefab, Random.onUnitSphere * 3f );

			// spheres have a hardLimit set so we need to null check before using them
			if( newObj )
			{
				newObj.transform.parent = transform;
				TrashMan.despawnAfterDelay( newObj, Random.Range( 5f, 8f ) );
			}
		}


		if( GUILayout.Button( "Spawn Light from Scene" ) )
		{
			var newObj = TrashMan.spawn( "light", Random.onUnitSphere * 10f );

			if( newObj )
			{
				newObj.transform.parent = transform;
				TrashMan.despawnAfterDelay( newObj, Random.Range( 5f, 8f ) );
			}
		}


		if( GUILayout.Button( "Spawn Particles by GameObject Name" ) )
		{
			TrashMan.spawn( "Particles", Random.onUnitSphere * 3f );
		}


		if( GUILayout.Button( "Create Recycle Bin at Runtime" ) )
		{
			_didCreateCapsuleRecycleBin = true;
			var recycleBin = new TrashManRecycleBin()
			{
				prefab = capsulePrefab
			};
			TrashMan.manageRecycleBin( recycleBin );
		}


		if( _didCreateCapsuleRecycleBin && GUILayout.Button( "Spawn Capsule" ) )
		{
			var newObj = TrashMan.spawn( capsulePrefab, Random.onUnitSphere * 5f, Random.rotation );
			TrashMan.despawnAfterDelay( newObj, Random.Range( 1f, 5f ) );
		}
	}

}
