using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class GLHandleVisuals_Disc : MonoBehaviour {
	public Material materialOverride;
	[Range (0.01f, 10)]

	public float gizmoResolution_relative = 1f;
	[Range (0.01f, 10)]
	public float gizmoSize_relative = 0.7f;
	[SerializeField]
	GLHandlesDrawer _GL_HandlesDrawer;
	//GLHandlesDrawer m_GLHandlesDrawer { get { if (_GL_HandlesDrawer == null) _GL_HandlesDrawer = GameObject.FindGameObjectWithTag ("GizmoDrawer").GetComponent<GLHandlesDrawer> (); return _GL_HandlesDrawer; } }
    GLHandlesDrawer m_GLHandlesDrawer { get { if (_GL_HandlesDrawer == null) _GL_HandlesDrawer = (GLHandlesDrawer)GameObject.FindObjectOfType(typeof(GLHandlesDrawer)); return _GL_HandlesDrawer; } }

	[SerializeField] private bool m_Render = true;

	public bool lookAtCamera = true;

	// Start is called before the first frame update
	void OnEnable () {
		//if (_GL_HandlesDrawer == null)
		//	_GL_HandlesDrawer = GameObject.FindGameObjectWithTag ("GizmoDrawer").GetComponent<GLHandlesDrawer> ();
		//    #if UNITY_EDITOR
		// 		if(!Application.isPlaying)
		// 			m_GLHandlesDrawer = (GLHandlesDrawer)FindObjectOfType(typeof(GLHandlesDrawer));
		// 	#endif
	}

	public void DrawGizmo (Transform cam, float gizmoGroupThicknessRelative, float gizmoGroupResolutionRelative, Material materialOverride) {
		/*
		#if UNITY_EDITOR
		//color of gizmo
		Handles.color = Color.cyan;
		#else
		//color of gizmo
		Handles.color = gizmoRuntimeColor;
		#endif*/
		if (!m_Render || !gameObject.activeInHierarchy)
			return;

		//reference to the parent rail
		//VRBasics_Rail rail = transform.parent.gameObject.GetComponent<VRBasics_Rail>();
		Material matToUse = null;
		if (this.materialOverride != null)
			matToUse = this.materialOverride;
		else
		if (materialOverride != null)
			matToUse = materialOverride;

		//draw a solid dot at the joint
		//Handles.DrawSolidDisc (transform.position, rail.transform.right, 0.01f);
#if UNITY_EDITOR
		if (!Application.isPlaying)
			cam = SceneView.currentDrawingSceneView.camera.transform;
#endif
		
		if (cam != null && lookAtCamera) {

			m_GLHandlesDrawer.DrawDiscBatched (
				transform.position,
				cam.forward,
				cam.right,
				cam.up,
				transform.localScale.x * m_GLHandlesDrawer.defaultGizmoSize * gizmoSize_relative * gizmoGroupThicknessRelative, m_GLHandlesDrawer.defaultGizmoResolution * gizmoResolution_relative * gizmoGroupResolutionRelative,
				matToUse
			);
		} else {
			m_GLHandlesDrawer.DrawDiscBatched (
				transform.position,
				transform.forward,
				transform.right,
				transform.up,
				transform.localScale.x * m_GLHandlesDrawer.defaultGizmoSize * gizmoSize_relative * gizmoGroupThicknessRelative, m_GLHandlesDrawer.defaultGizmoResolution * gizmoResolution_relative * gizmoGroupResolutionRelative,
				matToUse
			);
		}

	}
}