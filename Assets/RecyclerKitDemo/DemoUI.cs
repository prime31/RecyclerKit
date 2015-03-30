using UnityEngine;
using System.Collections;
#if UNITY_4_6 || UNITY_5_0
using UnityEngine.UI;
#endif


public class DemoUI : MonoBehaviour
{
	// instance variables with links to the prefabs that we will be spawning
	public GameObject cubePrefab;
	public GameObject spherePrefab;
	public GameObject capsulePrefab;

	private bool _didCreateCapsuleRecycleBin;
#if UNITY_4_6 || UNITY_5_0
	private bool _didCreateUiStuff;
	GameObject canvasRoot;
	GameObject uiPrefab;
#endif

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

#if UNITY_4_6 || UNITY_5_0
		if( GUILayout.Button( "Spawn UI element" ) )
		{
			CreateCanvas();
			var go = TrashMan.spawn( uiPrefab, Vector2.zero );
			go.transform.SetParent(canvasRoot.transform, true);
			var rt = go.transform as RectTransform;
			rt.anchoredPosition = new Vector2(Random.Range (-380,380), Random.Range (-280,280));
			TrashMan.despawnAfterDelay( go, Random.Range( 1f, 5f ) );
		}
#endif
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

#if UNITY_4_6 || UNITY_5_0
	void CreateCanvas()
	{
		if(!_didCreateUiStuff)
		{
			_didCreateUiStuff = true;
			//Create the UI canvas game object
			canvasRoot = new GameObject("Canvas");
			var canvas = canvasRoot.AddComponent<Canvas>();
			canvas.renderMode = RenderMode.ScreenSpaceOverlay;
			var cs = canvasRoot.AddComponent<CanvasScaler>();
			cs.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
			cs.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
			cs.referenceResolution = new Vector2(800,600);

			//create our ui prefab
			uiPrefab = new GameObject("UItxt");
			uiPrefab.transform.position = new Vector3(1000,10000);
			var txt = uiPrefab.AddComponent<Text>();
			txt.font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
			txt.text = "Some text";
			txt.horizontalOverflow = HorizontalWrapMode.Overflow;
			txt.color = Color.white;
			txt.resizeTextForBestFit = true;

			//Make a recycle bin for it
			var recycleBin = new TrashManRecycleBin()
			{
				prefab = uiPrefab
			};
			TrashMan.manageRecycleBin( recycleBin );
		}
	}
#endif
}
