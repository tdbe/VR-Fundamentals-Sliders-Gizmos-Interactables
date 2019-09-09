using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class GLHandleVisuals_Group : MonoBehaviour {

    public Material materialOverride;
    [Range (0.01f, 10)]
    public float gizmoGroupThicknessRelative = 1;
    [Range (0.01f, 1)]
    public float gizmoGroup_ResolutionRelative = 0.11f;
    float m_gizmoGroupThicknessRelative_orig;
    [Range (0.01f, 10)]
    public float gizmoHoverSize = 1;
    [Range (0.01f, 10)]
    public float gizmoClickSize = 1;
    public GLHandleVisuals_Circle[] circles;
    public GLHandleVisuals_Disc[] discs;
    public GLHandleVisuals_Line[] lines;

    Coroutine m_coroutine;

    bool m_allowedToDrawGizmos = true;

    public bool m_Debug;

    [SerializeField]
    GLHandlesDrawer _GL_HandlesDrawer;
    //GLHandlesDrawer m_GLHandlesDrawer { get { if (_GL_HandlesDrawer == null) _GL_HandlesDrawer = GameObject.FindGameObjectWithTag ("GizmoDrawer").GetComponent<GLHandlesDrawer> (); return _GL_HandlesDrawer; } }
    GLHandlesDrawer m_GLHandlesDrawer { get { if (_GL_HandlesDrawer == null) _GL_HandlesDrawer = (GLHandlesDrawer)GameObject.FindObjectOfType(typeof(GLHandlesDrawer)); return _GL_HandlesDrawer; } }


    [HideInInspector]
    public Valve.VR.InteractionSystem.Interactable interactableLinkedTo;
    Valve.VR.InteractionSystem.SoundPlayOneshot oneshotSound;

    AudioClip[] waveFiles;
    public enum SoundTypes{
        hover,
        click
    }
    //public SoundTypes waveFilesOrderReadOnly;

    void Start(){
        ConfigureSound();

        m_gizmoGroupThicknessRelative_orig = gizmoGroupThicknessRelative;

    }

    //Sound handled by the SoundPlayOneshot script
    void ConfigureSound(){
        #if UNITY_EDITOR
            if(!Application.isPlaying)
                return;
        #endif

        if(oneshotSound == null)
            oneshotSound = GetComponent<Valve.VR.InteractionSystem.SoundPlayOneshot>();
        if(oneshotSound == null){
            oneshotSound = gameObject.AddComponent<Valve.VR.InteractionSystem.SoundPlayOneshot>();
        }
        if(waveFiles == null)
            waveFiles = AudioClipGroup.Instance.hoverClickSounds;
        oneshotSound.waveFiles = waveFiles;
        oneshotSound.playAllSoundsInArrayAtOnce = false;
    }

    public void SetMaterialOverride (Material newmat) {
        materialOverride = newmat;
    }

    public void EnableVisuals () {
        m_allowedToDrawGizmos = true;
    }

    public void DisableVisuals () {
        m_allowedToDrawGizmos = false;
    }

    public void SetGizmoGroupThicknessRelative (float value) {
        gizmoGroupThicknessRelative = value;
    }

    IEnumerator DoActionAfterSeconds (float time, System.Action act) {
        yield return new WaitForSeconds (time);
        act ();
    }

    public void Hovered () {
        if (m_coroutine != null)
            StopCoroutine (m_coroutine);
        gizmoGroupThicknessRelative = m_gizmoGroupThicknessRelative_orig * gizmoHoverSize * m_GLHandlesDrawer.gizmoHoverSize;
        if(interactableLinkedTo!=null && interactableLinkedTo.hoveringHand!=null){
            oneshotSound.thisAudioSource = interactableLinkedTo.hoveringHand.GetComponent<AudioSource>();
            oneshotSound.Play((int)SoundTypes.hover);
        }
    }

    public void UnHovered () {
        if (m_coroutine != null)
            StopCoroutine (m_coroutine);
        Reset ();
        //m_coroutine = StartCoroutine(DoActionAfterSeconds( m_GLHandlesDrawer.interactionResetTimeShort, ()=>{ Reset();}));
    }

    public void Clicked () {
        gizmoGroupThicknessRelative = m_gizmoGroupThicknessRelative_orig * gizmoClickSize * m_GLHandlesDrawer.gizmoClickSize;
        if (m_coroutine != null)
            StopCoroutine (m_coroutine);
        if (gameObject.activeInHierarchy)
            m_coroutine = StartCoroutine (DoActionAfterSeconds (m_GLHandlesDrawer.interactionResetTimeShort, () => { Hovered (); }));
        if(interactableLinkedTo!=null && interactableLinkedTo.hoveringHand!=null){
            oneshotSound.thisAudioSource = interactableLinkedTo.hoveringHand.GetComponent<AudioSource>();
            oneshotSound.Play((int)SoundTypes.click);
        }
    }

    public void UnClicked () {
        if (m_coroutine != null)
            StopCoroutine (m_coroutine);
        if (gameObject.activeInHierarchy)
            m_coroutine = StartCoroutine (DoActionAfterSeconds (m_GLHandlesDrawer.interactionResetTimeShort, () => { Reset (); }));
    }

    public void Reset () {
        if (m_coroutine != null)
            StopCoroutine (m_coroutine);
        if (m_Debug) {
            //Debug.Log ("Called reset to " + m_gizmoGroupThicknessRelative_orig + " was " + gizmoGroupThicknessRelative);
        }
        gizmoGroupThicknessRelative = m_gizmoGroupThicknessRelative_orig;
    }



    void OnEnable () {
        //if (_GL_HandlesDrawer == null)
        //    _GL_HandlesDrawer = GameObject.FindGameObjectWithTag ("GizmoDrawer").GetComponent<GLHandlesDrawer> ();
        //    #if UNITY_EDITOR
        // 		if(!Application.isPlaying)
        // 			m_GLHandlesDrawer = (GLHandlesDrawer)FindObjectOfType(typeof(GLHandlesDrawer));
        // 	#endif

        circles = GetComponentsInChildren<GLHandleVisuals_Circle> ();
        discs = GetComponentsInChildren<GLHandleVisuals_Disc> ();
        lines = GetComponentsInChildren<GLHandleVisuals_Line> ();

        m_allowedToDrawGizmos = true;

        if(m_gizmoGroupThicknessRelative_orig==0)
            m_gizmoGroupThicknessRelative_orig = 1;
        Reset ();
    }

    void OnDisable () {

        m_allowedToDrawGizmos = false;
    }

#if UNITY_EDITOR

    void OnDrawGizmos () {
        if (!m_allowedToDrawGizmos)
            return;
        //if(Application.isPlaying)
        //    return;

        circles = GetComponentsInChildren<GLHandleVisuals_Circle> ();
        discs = GetComponentsInChildren<GLHandleVisuals_Disc> ();
        lines = GetComponentsInChildren<GLHandleVisuals_Line> ();

        for (int i = 0; i < circles.Length; i++) {

            circles[i].DrawGizmo (m_GLHandlesDrawer.cam, gizmoGroupThicknessRelative, gizmoGroup_ResolutionRelative, materialOverride);
        }
        for (int i = 0; i < discs.Length; i++) {
            discs[i].DrawGizmo (m_GLHandlesDrawer.cam, gizmoGroupThicknessRelative, gizmoGroup_ResolutionRelative, materialOverride);
        }
        for (int i = 0; i < lines.Length; i++) {
            lines[i].DrawGizmo (materialOverride);
        }
    }
#endif

 
    public void Update () {

        if (!m_allowedToDrawGizmos)
            return;

#if UNITY_EDITOR
        if (!Application.isPlaying)
            return;

        if (!Application.isPlaying)
            circles = GetComponentsInChildren<GLHandleVisuals_Circle> ();
        discs = GetComponentsInChildren<GLHandleVisuals_Disc> ();
        lines = GetComponentsInChildren<GLHandleVisuals_Line> ();
#endif

        for (int i = 0; i < circles.Length; i++) {
            circles[i].DrawGizmo (m_GLHandlesDrawer.cam, gizmoGroupThicknessRelative, gizmoGroup_ResolutionRelative, materialOverride);
        }
        for (int i = 0; i < discs.Length; i++) {
            discs[i].DrawGizmo (m_GLHandlesDrawer.cam, gizmoGroupThicknessRelative, gizmoGroup_ResolutionRelative, materialOverride);

        }
        for (int i = 0; i < lines.Length; i++) {
            lines[i].DrawGizmo (materialOverride);
        }

    }

}