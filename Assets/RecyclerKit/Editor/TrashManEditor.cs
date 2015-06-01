using UnityEditor;
using UnityEngine;
using System.Collections.Generic;


[CustomEditor( typeof( TrashMan ) )]
public class TrashManEditor : Editor
{
	/// <summary>
	/// True for indices corresponding to individual TrashManCan foldouts which should be expanded.
	/// </summary>
	private List<bool> _prefabFoldouts;
	private TrashMan _trashManTarget;

	private GUIStyle _boxStyle;
	private GUIStyle boxStyle
	{
		get
		{
			if( _boxStyle == null )
			{
				_boxStyle = new GUIStyle( GUI.skin.box );

				var tex = new Texture2D( 1, 1 );
				tex.hideFlags = HideFlags.HideAndDontSave;
				tex.SetPixel( 0, 0, Color.white );
				tex.Apply();

				_boxStyle.normal.background = tex;

				tex = new Texture2D( 1, 1 );
				tex.hideFlags = HideFlags.HideAndDontSave;
				tex.SetPixel( 0, 0, Color.green );
				tex.Apply();

				_boxStyle.hover.background = tex;

				var p = _boxStyle.padding;
				p.left = p.right = 20;
				p.top = 6;
				_boxStyle.padding = p;

				_boxStyle.fontSize = 15;
			}

			return _boxStyle;
		}
	}


	private GUIStyle _binStyleEven;
	private GUIStyle binStyleEven
	{
		get
		{
			if( _binStyleEven == null )
			{
				_binStyleEven = new GUIStyle( GUI.skin.box );

				var tex = new Texture2D( 1, 1 );
				tex.hideFlags = HideFlags.HideAndDontSave;
				tex.SetPixel( 0, 0, new Color( 1f, 1f, 1f, 0.2f ) );
				tex.Apply();

				_binStyleEven.normal.background = tex;
				var p = _binStyleEven.padding;
				p.left = 14;
				_binStyleEven.padding = p;
			}

			return _binStyleEven;
		}
	}


	private GUIStyle _binStyleOdd;
	private GUIStyle binStyleOdd
	{
		get
		{
			if( _binStyleOdd == null )
			{
				_binStyleOdd = new GUIStyle( GUI.skin.box );

				var tex = new Texture2D( 1, 1 );
				tex.hideFlags = HideFlags.HideAndDontSave;
				tex.SetPixel( 0, 0, new Color( 1f, 1f, 1f, 0.1f ) );
				tex.Apply();

				_binStyleOdd.normal.background = tex;
				var p = _binStyleEven.padding;
				p.left = 14;
				_binStyleOdd.padding = p;
			}

			return _binStyleOdd;
		}
	}


	private GUIStyle _buttonStyle;
	private GUIStyle buttonStyle
	{
		get
		{
			if( _buttonStyle == null )
			{
				_buttonStyle = new GUIStyle( GUI.skin.button );

				var normalTex = new Texture2D( 1, 1 );
				normalTex.hideFlags = HideFlags.HideAndDontSave;
				normalTex.SetPixel( 0, 0, new Color( 1f, 1f, 1f, 0.8f ) );
				normalTex.Apply();

				var activeTex = new Texture2D( 1, 1 );
				activeTex.hideFlags = HideFlags.HideAndDontSave;
				activeTex.SetPixel( 0, 0, Color.yellow );
				activeTex.Apply();

				var hoverTex = new Texture2D( 1, 1 );
				hoverTex.hideFlags = HideFlags.HideAndDontSave;
				hoverTex.SetPixel( 0, 0, Color.red );
				hoverTex.Apply();

				_buttonStyle.normal.background = normalTex;
				_buttonStyle.hover.background = hoverTex;
				_buttonStyle.active.background = activeTex;

				_buttonStyle.normal.textColor = Color.black;
			}

			return _buttonStyle;
		}
	}


	#region Methods

	public void OnEnable()
	{
		_trashManTarget = target as TrashMan;
		_trashManTarget.recycleBinCollection = (target as TrashMan).recycleBinCollection;

		_prefabFoldouts = new List<bool>();
		if( _trashManTarget.recycleBinCollection != null )
			for( int n = 0; n < _trashManTarget.recycleBinCollection.Count; n++ )
				_prefabFoldouts.Add( true );

		clearNullReferences();
	}


	void OnDisable()
	{
		if( _boxStyle != null )
		{
			DestroyImmediate( _boxStyle.normal.background );
			DestroyImmediate( _boxStyle.hover.background );
		}

		if( _binStyleEven != null )
			DestroyImmediate( _binStyleEven.normal.background );

		if( _binStyleOdd != null )
			DestroyImmediate( _binStyleOdd.normal.background );

		if( _buttonStyle != null )
		{
			DestroyImmediate( _buttonStyle.normal.background );
			DestroyImmediate( _buttonStyle.hover.background );
			DestroyImmediate( _buttonStyle.active.background );
		}

		_boxStyle = null;
		_binStyleEven = null;
		_binStyleOdd = null;
		_buttonStyle = null;
	}


	/// <summary>
	/// null checks the TrashMans List and removes any cans with no prefab found. This will clear out any GameObjects
	/// from the scene that no longer exist
	/// </summary>
	private void clearNullReferences()
	{
		if( _trashManTarget.recycleBinCollection == null )
			return;

		int n = 0;
		while( n < _trashManTarget.recycleBinCollection.Count )
		{
			if( _trashManTarget.recycleBinCollection[n].prefab == null )
				_trashManTarget.recycleBinCollection.RemoveAt( _trashManTarget.recycleBinCollection.Count - 1 );
			else
				n++;
		}
	}


	/// <summary>
	/// adds a new bin to the TrashMan collection
	/// </summary>
	/// <param name="go">Go.</param>
	private void addRecycleBin( GameObject go )
	{
		if( _trashManTarget.recycleBinCollection == null )
			_trashManTarget.recycleBinCollection = new List<TrashManRecycleBin>();

		if( _trashManTarget.recycleBinCollection != null )
		{
			foreach( var recycleBin in _trashManTarget.recycleBinCollection )
			{
				if( recycleBin.prefab.gameObject.name == go.name )
				{
					EditorUtility.DisplayDialog( "Trash Man", "Trash Man already manages a GameObject with the name '" + go.name + "'.\n\nIf you are attempting to manage multiple GameObjects sharing the same name, you will need to first give them unique names.", "OK" );
					return;
				}
			}
		}

		var newPrefabPool = new TrashManRecycleBin();
		newPrefabPool.prefab = go;

		_trashManTarget.recycleBinCollection.Add( newPrefabPool );
		while( _trashManTarget.recycleBinCollection.Count > _prefabFoldouts.Count )
			_prefabFoldouts.Add( false );
	}


	public override void OnInspectorGUI()
	{
		if( Application.isPlaying )
		{
			if( _prefabFoldouts.Count < _trashManTarget.recycleBinCollection.Count )
			{
				for( var i = 0; i < _trashManTarget.recycleBinCollection.Count - _prefabFoldouts.Count; i++ )
					_prefabFoldouts.Add( false );
			}
			//base.OnInspectorGUI();
			//return;
		}

		base.OnInspectorGUI();

		GUILayout.Space( 15f );
		dropAreaGUI();

		if( _trashManTarget.recycleBinCollection == null )
			return;

		GUILayout.Space( 5f );
		GUILayout.Label( "Recycle Bins", EditorStyles.boldLabel );

		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.BeginVertical();

		for( int n = 0; n < _trashManTarget.recycleBinCollection.Count; n++ )
		{
			var prefabPool = _trashManTarget.recycleBinCollection[n];

			// wrapper vertical allows us to style each element
			EditorGUILayout.BeginVertical( n % 2 == 0 ? binStyleEven : binStyleOdd );

			// PrefabPool DropDown
			EditorGUILayout.BeginHorizontal();
			_prefabFoldouts[n] = EditorGUILayout.Foldout( _prefabFoldouts[n], prefabPool.prefab.name, EditorStyles.foldout );
			if( GUILayout.Button( "X", buttonStyle, GUILayout.Width( 20f ), GUILayout.Height( 15f ) ) && EditorUtility.DisplayDialog( "Remove Recycle Bin", "Are you sure you want to remove this recycle bin?", "Yes", "Cancel" ) )
				_trashManTarget.recycleBinCollection.RemoveAt( n );
			EditorGUILayout.EndHorizontal();

			GUILayout.Space( 2 );

			if( _prefabFoldouts[n] )
			{
				EditorGUILayout.BeginHorizontal();
				GUILayout.Space( 10f );
				EditorGUILayout.BeginVertical();

				// PreAlloc
				EditorGUILayout.BeginHorizontal();
				GUILayout.Label( new GUIContent( "Preallocate Count", "Total items to create at scene start" ), EditorStyles.label, GUILayout.Width( 115f ) );
				prefabPool.instancesToPreallocate = EditorGUILayout.IntField( prefabPool.instancesToPreallocate );
				if( prefabPool.instancesToPreallocate < 0 )
					prefabPool.instancesToPreallocate = 0;
				EditorGUILayout.EndHorizontal();

				// AllocBlock. only valid is prefabPool.imposeHardLimit is false
				EditorGUI.BeginDisabledGroup( prefabPool.imposeHardLimit );
				{
					EditorGUILayout.BeginHorizontal();
					GUILayout.Label( new GUIContent( "Allocate Block Count", "Once the bin limit is reached, this is how many new objects will be created as necessary" ), EditorStyles.label, GUILayout.Width( 115f ) );
					prefabPool.instancesToAllocateIfEmpty = EditorGUILayout.IntField( prefabPool.instancesToAllocateIfEmpty );
					if( prefabPool.instancesToAllocateIfEmpty < 1 )
						prefabPool.instancesToAllocateIfEmpty = 1;
					EditorGUILayout.EndHorizontal();
				}
				EditorGUI.EndDisabledGroup();


				// automaticallyRecycleParticleSystems
				EditorGUILayout.BeginHorizontal();
				GUILayout.Label( new GUIContent( "Recycle ParticleSystems", "If true, the GameObject must contain a ParticleSystem! It will be automatically despawned after system.duration." ), EditorStyles.label, GUILayout.Width( 115f ) );
				prefabPool.automaticallyRecycleParticleSystems = EditorGUILayout.Toggle( prefabPool.automaticallyRecycleParticleSystems );
				EditorGUILayout.EndHorizontal();


				// HardLimit
				EditorGUILayout.BeginHorizontal();
				GUILayout.Label( new GUIContent( "Enable Hard Limit ", "If true, the bin will return null if a new item is requested and the Limit was reached" ), EditorStyles.label, GUILayout.Width( 115f ) );
				prefabPool.imposeHardLimit = EditorGUILayout.Toggle( prefabPool.imposeHardLimit );
				EditorGUILayout.EndHorizontal();

				if( prefabPool.imposeHardLimit )
				{
					EditorGUILayout.BeginHorizontal();
					GUILayout.Space( 20f );
					GUILayout.Label( new GUIContent( "Limit", "Max number of items allowed in the bin when Hard Limit is true" ), EditorStyles.label, GUILayout.Width( 100f ) );
					prefabPool.hardLimit = EditorGUILayout.IntField( prefabPool.hardLimit );
					if( prefabPool.hardLimit < 1 )
						prefabPool.hardLimit = 1;
					EditorGUILayout.EndHorizontal();
				}


				EditorGUILayout.BeginHorizontal();
				GUILayout.Label( new GUIContent( "Enable Culling", "If true, items in excess of Cull Above will be destroyed automatically" ), EditorStyles.label, GUILayout.Width( 115f ) );
				prefabPool.cullExcessPrefabs = EditorGUILayout.Toggle( prefabPool.cullExcessPrefabs );
				EditorGUILayout.EndHorizontal();


				if( prefabPool.cullExcessPrefabs )
				{
					EditorGUILayout.BeginHorizontal();
					GUILayout.Space( 20f );
					GUILayout.Label( new GUIContent( "Cull Above", "Max number of items to allow. If item count exceeds this they will be culled" ), EditorStyles.label, GUILayout.Width( 100f ) );
					prefabPool.instancesToMaintainInPool = EditorGUILayout.IntField( prefabPool.instancesToMaintainInPool );
					if( prefabPool.instancesToMaintainInPool < 0 )
						prefabPool.instancesToMaintainInPool = 0;
					if( prefabPool.imposeHardLimit && prefabPool.instancesToMaintainInPool > prefabPool.hardLimit )
						prefabPool.instancesToMaintainInPool = prefabPool.hardLimit;
					EditorGUILayout.EndHorizontal();
					EditorGUILayout.BeginHorizontal();
					GUILayout.Space( 20f );
					GUILayout.Label( new GUIContent( "Cull Delay", "Duration in seconds between cull checks. Note that the master cull check only occurs every 5 seconds" ), EditorStyles.label, GUILayout.Width( 100f ) );
					prefabPool.cullInterval = EditorGUILayout.FloatField( prefabPool.cullInterval );
					if( prefabPool.cullInterval < 0 )
						prefabPool.cullInterval = 0;
					EditorGUILayout.EndHorizontal();
				}

				if( _trashManTarget.persistBetweenScenes )
				{
					EditorGUILayout.BeginHorizontal();
					GUILayout.Label( new GUIContent( "Persist Between Scenes", "If true this recycle bin will not be purged when a new level loads" ), EditorStyles.label, GUILayout.Width( 115f ) );
					prefabPool.persistBetweenScenes = EditorGUILayout.Toggle( prefabPool.persistBetweenScenes );
					EditorGUILayout.EndHorizontal();
				}

				EditorGUILayout.EndVertical();
				EditorGUILayout.EndHorizontal();
			}


			EditorGUILayout.EndVertical();
			EditorGUILayout.Space();
		}

		EditorGUILayout.EndHorizontal();
		EditorGUILayout.EndVertical();

		if( GUI.changed )
			EditorUtility.SetDirty( target );
	}


	private void dropAreaGUI()
	{
		var evt = Event.current;
		var dropArea = GUILayoutUtility.GetRect( 0f, 60f, GUILayout.ExpandWidth( true ) );
		GUI.Box( dropArea, "Drop a Prefab or GameObject here to create a new can in your TrashMan", boxStyle );

		switch( evt.type )
		{
			case EventType.DragUpdated:
			case EventType.DragPerform:
			{
				if( !dropArea.Contains( evt.mousePosition ) )
					break;

				DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

				if( evt.type == EventType.DragPerform )
				{
					DragAndDrop.AcceptDrag();
					foreach( var draggedObject in DragAndDrop.objectReferences )
					{
						var go = draggedObject as GameObject;
						if( !go )
							continue;

						// TODO: perhaps we should only allow prefabs or perhaps allow GO's in the scene as well?
						// uncomment to allow only prefabs
//						if( PrefabUtility.GetPrefabType( go ) == PrefabType.None )
//						{
//							EditorUtility.DisplayDialog( "Trash Man", "Trash Man cannot manage the object '" + go.name + "' as it is not a prefab.", "OK" );
//							continue;
//						}

						addRecycleBin( go );
					}
				}

				Event.current.Use();
				break;
			} // end DragPerform
		} // end switch
	}

	#endregion

}