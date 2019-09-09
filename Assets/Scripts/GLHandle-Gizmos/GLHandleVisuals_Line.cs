using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class GLHandleVisuals_Line : MonoBehaviour {
    public Material materialOverride;
    public Transform PointA;
    public Transform PointB;
    //[Range(0.01f,10)]
    //public float gizmoResolution_relative = 1f;
    //[Range(0.01f,10)]
    //public float gizmoSize_relative = 1f;
    [SerializeField]
    GLHandlesDrawer _GL_HandlesDrawer;
    GLHandlesDrawer m_GLHandlesDrawer { get { if (_GL_HandlesDrawer == null) _GL_HandlesDrawer = (GLHandlesDrawer)GameObject.FindObjectOfType(typeof(GLHandlesDrawer)); return _GL_HandlesDrawer; } }

    [SerializeField] private bool m_Render;

    

    // Start is called before the first frame update
    void OnEnable () {
        //if (_GL_HandlesDrawer == null)
        //    _GL_HandlesDrawer = GameObject.FindGameObjectWithTag ("GizmoDrawer").GetComponent<GLHandlesDrawer> ();
        //    #if UNITY_EDITOR
        // 		if(!Application.isPlaying)
        // 			m_GLHandlesDrawer = (GLHandlesDrawer)FindObjectOfType(typeof(GLHandlesDrawer));
        // 	#endif

    }

    public void DrawGizmo (Material materialOverride) {
        if (!m_Render || !gameObject.activeInHierarchy)
            return;
        if (PointA != null && PointB != null) {
            //draw a line
            //Handles.DrawLine (minLimitPos, maxLimitPos);

            if (this.materialOverride != null) {
                //Debug.Log("Drawing "+this.name+" with local material: "+this.materialOverride);
                m_GLHandlesDrawer.DrawLineBatched (PointA.position, PointB.position, this.materialOverride);
            } else
            if (materialOverride != null) {
                //Debug.Log("Drawing "+this.name+" with group material: "+materialOverride);
                m_GLHandlesDrawer.DrawLineBatched (PointA.position, PointB.position, materialOverride);
            } else {
                //Debug.Log("Drawing "+this.name+" with global material.");
                m_GLHandlesDrawer.DrawLineBatched (PointA.position, PointB.position, null);
            }
        }

        //remove the dummy
        //DestroyImmediate (dummyTrans);

    }
}