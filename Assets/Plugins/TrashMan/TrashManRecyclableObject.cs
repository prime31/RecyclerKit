using UnityEngine;
using System.Collections;



public class TrashManRecyclableObject : MonoBehaviour
{
	//[HideInInspector]
	public int prefabInstanceId;
	public event System.Action onSpawnedEvent;
	public event System.Action onDespawnedEvent;

	
	public void onSpawned()
	{
		if( onSpawnedEvent != null )
			onSpawnedEvent();
	}


	public void onDespawned()
	{
		if( onDespawnedEvent != null )
			onDespawnedEvent();
	}

}
