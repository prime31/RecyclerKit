using UnityEngine;
using System.Collections;



public class DemoUI : MonoBehaviour
{
	public GameObject cubePrefab;
	public GameObject spherePrefab;
	public GameObject capsulePrefab;


	void OnGUI()
	{
		if( GUILayout.Button( "Spawn Cube" ) )
		{
			var newObj = TrashMan.spawn( cubePrefab.GetComponent<TrashManRecyclableObject>(), Random.onUnitSphere * 5f, Random.rotation );
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
				newObj.light.color = new Color( Random.Range( 0, 1f ), Random.Range( 0, 1f ), Random.Range( 0, 1f ) );
				newObj.transform.parent = transform;
				TrashMan.despawnAfterDelay( newObj, Random.Range( 5f, 8f ) );
			}
		}


		if( GUILayout.Button( "Spawn Particles by GameObject Name" ) )
		{
			var newObj = TrashMan.spawn( "Particles", Random.onUnitSphere * 3f );
			TrashMan.despawnAfterDelay( newObj, Random.Range( 2f, 5f ) );
		}


		if( GUILayout.Button( "Create Recycle Bin at Runtime" ) )
		{
			var recycleBin = new TrashManRecycleBin( capsulePrefab.GetComponent<TrashManRecyclableObject>() )
			{
				instancesToPreallocate = 3
			};
			TrashMan.manageRecycleBin( recycleBin );
		}


		if( GUILayout.Button( "Spawn Capsule" ) )
		{
			var newObj = TrashMan.spawn( capsulePrefab, Random.onUnitSphere * 5f, Random.rotation );
			TrashMan.despawnAfterDelay( newObj, Random.Range( 1f, 5f ) );
		}
	}

}
