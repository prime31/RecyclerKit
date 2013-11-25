using UnityEngine;
using System.Collections;



public class DemoUI : MonoBehaviour
{
	public GameObject cubePrefab;
	public GameObject spherePrefab;


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

		if( GUILayout.Button( "" ) )
		{
			
		}

		if( GUILayout.Button( "" ) )
		{
			
		}

		if( GUILayout.Button( "" ) )
		{
			
		}
	}

}
