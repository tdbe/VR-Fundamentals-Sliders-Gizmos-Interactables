//======= Copyright (c) Valve Corporation, All rights reserved. ===============
//
// Purpose: Single location that the player can teleport to
//
//=============================================================================

using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Valve.VR.InteractionSystem
{
	//-------------------------------------------------------------------------
	public class TeleportPoint : TeleportMarkerBase
	{
		public enum TeleportPointType
		{
			MoveToLocation,
			SwitchToNewScene
		};

		//Public variables
		public TeleportPointType teleportType = TeleportPointType.MoveToLocation;
		public string title;
		public string switchToScene;
		public Color titleVisibleColor;
		public Color titleHighlightedColor;
		public Color titleLockedColor;
		public bool playerSpawnPoint = false;

		//Private data
		private bool gotReleventComponents = false;
		[SerializeField]
		private MeshRenderer markerMesh;
		[SerializeField]
		private MeshRenderer switchSceneIcon;
		[SerializeField]
		private MeshRenderer moveLocationIcon;
		[SerializeField]
		private MeshRenderer lockedIcon;
		[SerializeField]
		private MeshRenderer pointIcon;
		[SerializeField]
		private Transform lookAtJointTransform;
		[SerializeField]

		private new Animation animation;
		[SerializeField]
		private Text titleText;
		private Player player;
		private Vector3 lookAtPosition = Vector3.zero;
		private int tintColorID = 0;
		private Color tintColor = Color.clear;
		private Color titleColor = Color.clear;
		private float fullTitleAlpha = 0.0f;

		//Constants
		private const string switchSceneAnimation = "switch_scenes_idle";
		private const string moveLocationAnimation = "move_location_idle";
		private const string lockedAnimation = "locked_idle";


		//-------------------------------------------------
		public override bool showReticle
		{
			get
			{
				return false;
			}
		}


		//-------------------------------------------------
		void Awake()
		{
			GetRelevantComponents();

			if(!animation)
				animation = GetComponentInChildren<Animation>();

			tintColorID = Shader.PropertyToID( "_TintColor" );

			if(moveLocationIcon) moveLocationIcon.gameObject.SetActive( false );
			if(switchSceneIcon) switchSceneIcon.gameObject.SetActive( false );
			if(lockedIcon) lockedIcon.gameObject.SetActive( false );

			UpdateVisuals();
		}


		//-------------------------------------------------
		void Start()
		{
			player = Player.instance;
		}


		//-------------------------------------------------
		void Update()
		{
			if(lookAtJointTransform) {
				if ( Application.isPlaying )
				{
					lookAtPosition.x = player.hmdTransform.position.x;
					lookAtPosition.y = lookAtJointTransform.position.y;
					lookAtPosition.z = player.hmdTransform.position.z;

					lookAtJointTransform.LookAt( lookAtPosition );
				}
			}
		}


		//-------------------------------------------------
		public override bool ShouldActivate( Vector3 playerPosition )
		{
			return ( Vector3.Distance( transform.position, playerPosition ) > 1.0f );
		}


		//-------------------------------------------------
		public override bool ShouldMovePlayer()
		{
			return true;
		}


		//-------------------------------------------------
		public override void Highlight( bool highlight )
		{
			if ( !locked )
			{
				if ( highlight )
				{
					SetMeshMaterials( Valve.VR.InteractionSystem.Teleport.instance.pointHighlightedMaterial, titleHighlightedColor );
				}
				else
				{
					SetMeshMaterials( Valve.VR.InteractionSystem.Teleport.instance.pointVisibleMaterial, titleVisibleColor );
				}
			}

			if ( highlight )
			{
				if(pointIcon) pointIcon.gameObject.SetActive( true );
				if(animation) animation.Play();
			}
			else
			{
				if(pointIcon) pointIcon.gameObject.SetActive( false );
				if(animation) animation.Stop();
			}
		}


		//-------------------------------------------------
		public override void UpdateVisuals()
		{
			if ( !gotReleventComponents )
			{
				return;
			}

			if ( locked )
			{
				SetMeshMaterials( Valve.VR.InteractionSystem.Teleport.instance.pointLockedMaterial, titleLockedColor );

				if(pointIcon) pointIcon = lockedIcon;

				if(animation) animation.clip = animation.GetClip( lockedAnimation );
			}
			else
			{
				SetMeshMaterials( Valve.VR.InteractionSystem.Teleport.instance.pointVisibleMaterial, titleVisibleColor );

				switch ( teleportType )
				{
					case TeleportPointType.MoveToLocation:
						{
							if(pointIcon) pointIcon = moveLocationIcon;

							if(animation) animation.clip = animation.GetClip( moveLocationAnimation );
						}
						break;
					case TeleportPointType.SwitchToNewScene:
						{
							if(pointIcon) pointIcon = switchSceneIcon;

							if(animation) animation.clip = animation.GetClip( switchSceneAnimation );
						}
						break;
				}
			}

			if(titleText) titleText.text = title;
		}


		//-------------------------------------------------
		public override void SetAlpha( float tintAlpha, float alphaPercent )
		{
			tintColor = markerMesh.material.GetColor( tintColorID );
			tintColor.a = tintAlpha;

			markerMesh.material.SetColor( tintColorID, tintColor );
			if(switchSceneIcon) switchSceneIcon.material.SetColor( tintColorID, tintColor );
			if(moveLocationIcon) moveLocationIcon.material.SetColor( tintColorID, tintColor );
			if(lockedIcon) lockedIcon.material.SetColor( tintColorID, tintColor );

			titleColor.a = fullTitleAlpha * alphaPercent;
			if(titleText) titleText.color = titleColor;
		}


		//-------------------------------------------------
		public void SetMeshMaterials( Material material, Color textColor )
		{
			markerMesh.material = material;
			if(switchSceneIcon) switchSceneIcon.material = material;
			if(moveLocationIcon) moveLocationIcon.material = material;
			if(lockedIcon) lockedIcon.material = material;

			titleColor = textColor;
			fullTitleAlpha = textColor.a;
			if(titleText) titleText.color = titleColor;
		}


		//-------------------------------------------------
		public void TeleportToScene()
		{
			if ( !string.IsNullOrEmpty( switchToScene ) )
			{
				Debug.Log("<b>[SteamVR Interaction]</b> TeleportPoint: Hook up your level loading logic to switch to new scene: " + switchToScene );
			}
			else
			{
				Debug.LogError("<b>[SteamVR Interaction]</b> TeleportPoint: Invalid scene name to switch to: " + switchToScene );
			}
		}


		//-------------------------------------------------
		public void GetRelevantComponents()
		{

			// markerMesh = transform.Find( "teleport_marker_mesh" ).GetComponent<MeshRenderer>();
			// switchSceneIcon = transform.Find( "teleport_marker_lookat_joint/teleport_marker_icons/switch_scenes_icon" ).GetComponent<MeshRenderer>();
			// moveLocationIcon = transform.Find( "teleport_marker_lookat_joint/teleport_marker_icons/move_location_icon" ).GetComponent<MeshRenderer>();
			// lockedIcon = transform.Find( "teleport_marker_lookat_joint/teleport_marker_icons/locked_icon" ).GetComponent<MeshRenderer>();
			// lookAtJointTransform = transform.Find( "teleport_marker_lookat_joint" );

			// titleText = transform.Find( "teleport_marker_lookat_joint/teleport_marker_canvas/teleport_marker_canvas_text" ).GetComponent<Text>();

			gotReleventComponents = true;
		}


		//-------------------------------------------------
		public void ReleaseRelevantComponents()
		{
			// markerMesh = null;
			// switchSceneIcon = null;
			// moveLocationIcon = null;
			// lockedIcon = null;
			// lookAtJointTransform = null;
			// titleText = null;
		}


		//-------------------------------------------------
		public void UpdateVisualsInEditor()
		{
			if ( Application.isPlaying )
			{
				return;
			}

			GetRelevantComponents();

			if ( locked )
			{
				if(lockedIcon)
					lockedIcon.gameObject.SetActive( true );
				if(moveLocationIcon)
					moveLocationIcon.gameObject.SetActive( false );
				if(switchSceneIcon)
					switchSceneIcon.gameObject.SetActive( false );

				markerMesh.sharedMaterial = Valve.VR.InteractionSystem.Teleport.instance.pointLockedMaterial;
				if(lockedIcon) lockedIcon.sharedMaterial = Valve.VR.InteractionSystem.Teleport.instance.pointLockedMaterial;

				if(titleText) titleText.color = titleLockedColor;
			}
			else
			{
				if(lockedIcon) lockedIcon.gameObject.SetActive( false );

				markerMesh.sharedMaterial = Valve.VR.InteractionSystem.Teleport.instance.pointVisibleMaterial;
				if(switchSceneIcon) switchSceneIcon.sharedMaterial = Valve.VR.InteractionSystem.Teleport.instance.pointVisibleMaterial;
				if(moveLocationIcon) moveLocationIcon.sharedMaterial = Valve.VR.InteractionSystem.Teleport.instance.pointVisibleMaterial;

				if(titleText) titleText.color = titleVisibleColor;

				switch ( teleportType )
				{
					case TeleportPointType.MoveToLocation:
						{
							if(moveLocationIcon) moveLocationIcon.gameObject.SetActive( true );
							if(switchSceneIcon) switchSceneIcon.gameObject.SetActive( false );
						}
						break;
					case TeleportPointType.SwitchToNewScene:
						{
							if(moveLocationIcon) moveLocationIcon.gameObject.SetActive( false );
							if(switchSceneIcon) switchSceneIcon.gameObject.SetActive( true );
						}
						break;
				}
			}

			if(titleText) titleText.text = title;

			ReleaseRelevantComponents();
		}
	}


#if UNITY_EDITOR
	//-------------------------------------------------------------------------
	[CustomEditor( typeof( TeleportPoint ) )]
	public class TeleportPointEditor : Editor
	{
		//-------------------------------------------------
		void OnEnable()
		{
			if ( Selection.activeTransform )
			{
				TeleportPoint teleportPoint = Selection.activeTransform.GetComponent<TeleportPoint>();
                if (teleportPoint != null)
				    teleportPoint.UpdateVisualsInEditor();
			}
		}


		//-------------------------------------------------
		public override void OnInspectorGUI()
		{
			DrawDefaultInspector();

			if ( Selection.activeTransform )
			{
				TeleportPoint teleportPoint = Selection.activeTransform.GetComponent<TeleportPoint>();
				if ( GUI.changed )
				{
					teleportPoint.UpdateVisualsInEditor();
				}
			}
		}
	}
#endif
}
