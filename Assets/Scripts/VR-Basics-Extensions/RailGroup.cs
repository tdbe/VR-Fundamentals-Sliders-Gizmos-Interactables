#define USE_OCULUS
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/*
Sometimes you want a slider to follow a more complex segmented path.
This script takes all the VRBasics Rais, groups them, and exposes a slider that treats all children as one continuous slider.
*/
[ExecuteInEditMode]
public class RailGroup : InteractableValue
{
  

    //[Range(0,1)]
    //public new float sliderPosition;

    float prevPos;
    [System.Serializable]
    public class UnityInteractableValueEvent : UnityEvent<InteractableValue>
    {
    }
    [SerializeField]
    UnityInteractableValueEvent onSliderPositionUpdate;

    [SerializeField]
    UnityEvent onSliderBinaryOn;
    [SerializeField]
    UnityEvent onSliderBinaryOff;
    [SerializeField]
    UnityEvent onSliderOnMax;
    [SerializeField]
    UnityEvent onSliderOn_25Percent;
    [SerializeField]
    UnityEvent onSliderOn_75Percent;
    


    [SerializeField]
    int m_currentRail = 0;
    int m_currentRailOld = 0;

    [SerializeField]
    Transform m_railsParent;
    [SerializeField]
    VRBasics_Rail[] children;

    [SerializeField]
    Transform m_railGroupGlobalHandle;
    bool m_IsGlobalHandleGlobal; // checks if the user in the inspector set the m_railGroupGlobalHandle reference to an internal rail slider game object or an unrelated object.


    Coroutine m_coroutine;

    float m_minMechanicalSliderValue = 0.05f;
    float m_maxMechanicalSliderValue = 0.95f;

    AutoGrabInteractable m_agi;
    [SerializeField]
    bool m_useAutoGrabInteractable;

    public bool startAtZero;

    //float m_lastRailSwitchTime;
    //float m_railSwitchDebounceTime = 100f;

    Valve.VR.InteractionSystem.SoundPlayOneshot oneshotSound;

    AudioClip[] waveFiles;

    // Start is called before the first frame update
    void Start()
    {
        // checks if the user in the inspector set the m_railGroupGlobalHandle reference to an internal rail slider game object or an unrelated object.
        if(m_railGroupGlobalHandle !=null && m_railGroupGlobalHandle.parent.GetComponent<VRBasics_Rail>() == null){
            m_IsGlobalHandleGlobal = true;
        }

        if(onSliderPositionUpdate==null)
            onSliderPositionUpdate = new UnityInteractableValueEvent();
  

        children = m_railsParent.GetComponentsInChildren<VRBasics_Rail>();

        #if UNITY_EDITOR
            if(!Application.isPlaying)
                return;
#endif
        //m_railGroupGlobalHandle.parent = children[m_currentRail].transform.GetChild(0).GetChild(0);
        ToggleChildren(false);
        calculateGlobalSliderPosition();

        InitSound();
    }

    void InitSound(){
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

    void OnEnable(){
        m_agi = children[m_currentRail].slider.child.GetComponent<AutoGrabInteractable>();
      
    //    #if UNITY_EDITOR
	// 		if(!Application.isPlaying)
	// 			m_GLHandlesDrawer = (GLHandlesDrawer)FindObjectOfType(typeof(GLHandlesDrawer));
	// 	#endif
        if(startAtZero){
            StartCoroutine(DoActionAfterFrames());
        }
    }

    IEnumerator DoActionAfterFrames(){
        //Debug.Log("I reset this object: " + transform.parent.name);
        yield return new WaitForEndOfFrame();
        ResetInteractable();
        yield return new WaitForEndOfFrame();
        ResetInteractable();
        yield return new WaitForEndOfFrame();
        ResetInteractable();
    }

    void OnDisable() {
        if(m_agi!=null)
            m_agi.ManualLetGoOfObject();
    }

    public override void ResetInteractable(){
            children[m_currentRail].slider.transform.localPosition = Vector3.zero;
            children[m_currentRail].slider.position = 0;
            children[m_currentRail].slider.child.transform.localPosition = Vector3.zero;;//children[m_currentRail].slider.transform.position;
    }

    void calculateGlobalSliderPosition()
    {
        //sliderPosition = (m_currentRail+children[m_currentRail].slider.position)/children.Length;
        float totalRailLength = 0;
        float lengthSoFar = 0;
        for (int i = 0; i < children.Length; i++)
        {
            if (i < m_currentRail)
            {
                lengthSoFar += children[i].length;
            }
            else if (i == m_currentRail)
            {
                lengthSoFar += children[m_currentRail].length * children[m_currentRail].slider.position;
            }
            totalRailLength += children[i].length;
        }
        sliderPosition = lengthSoFar / totalRailLength;
        //Debug.Log("Calculated global to be this: " + sliderPosition + " for this object: " + transform.parent.name);
    }

    Vector3 getWorldPositionAtGlobalSliderPosition(float sliderPosition)
    {
        return children[m_currentRail].slider.child.transform.position;
    }

    void ToggleChildren(bool alsoGrab = true)
    {
        for (int i = 0; i < children.Length; i++)
        {
            if (i != m_currentRail)
            {
                Transform child = children[i].slider.child.transform;
                //children[i].slider.gameObject.SetActive(false);
                children[i].slider.enabled = false;
                if (child.childCount > 0)
                    child.GetChild(0).GetComponent<Collider>().enabled = false;
                else
                    child.GetComponent<Collider>().enabled = false;
                child.GetComponent<AutoGrabInteractable>().ManualLetGoOfObject();
            }
            else
            {
                Transform child = children[m_currentRail].slider.child.transform;
                //children[m_currentRail].slider.gameObject.SetActive(true);
                children[m_currentRail].slider.enabled = true;
                if(child.childCount>0)
                    child.GetChild(0).GetComponent<Collider>().enabled = true;
                else
                    child.GetComponent<Collider>().enabled = true;
                if(alsoGrab)
                    children[m_currentRail].slider.child.GetComponent<AutoGrabInteractable>().ManualGrabObject();
            }
        }
    }

    void LateUpdate(){
        m_currentRailOld = m_currentRail;
    }

    void Update(){
        calculateGlobalSliderPosition();
        if(m_railGroupGlobalHandle!=null && m_IsGlobalHandleGlobal)
            m_railGroupGlobalHandle.transform.position = getWorldPositionAtGlobalSliderPosition(sliderPosition);

        if (prevPos!=sliderPosition){
            if(onSliderPositionUpdate!=null){
                onSliderPositionUpdate.Invoke(this);
                if(oneshotSound!=null){
                    Valve.VR.InteractionSystem.Interactable interactable = children[m_currentRail].slider.child.GetComponent<Valve.VR.InteractionSystem.Interactable>();
                    if(interactable !=null){
                        if(interactable.attachedToHand !=null){
                            oneshotSound.thisAudioSource = interactable.attachedToHand.GetComponent<AudioSource>();
                            oneshotSound.Play((int)AudioClipGroup.SoundTypes_hoverClickSounds.hover);
                        }
                        else if(interactable.hoveringHand !=null){
                            oneshotSound.thisAudioSource = interactable.hoveringHand.GetComponent<AudioSource>();
                            oneshotSound.Play((int)AudioClipGroup.SoundTypes_hoverClickSounds.hover);
                        }
                    }
                }
            }

            if(sliderPosition >=.25f && prevPos<.25f && onSliderOn_25Percent!=null)
                onSliderOn_25Percent.Invoke();
            if(sliderPosition >=.75f && prevPos<.75f && onSliderOn_75Percent!=null)
                onSliderOn_75Percent.Invoke();

            if(onSliderBinaryOn!=null && sliderPosition>m_minMechanicalSliderValue && prevPos <= m_minMechanicalSliderValue){
                onSliderBinaryOn.Invoke();
            }
            else if(onSliderOnMax!=null && sliderPosition>m_maxMechanicalSliderValue && prevPos <= m_maxMechanicalSliderValue){
                onSliderOnMax.Invoke();
                sliderPosition = 1;
            }
            else if(onSliderBinaryOff!=null && sliderPosition<=m_minMechanicalSliderValue && prevPos > m_minMechanicalSliderValue){
                onSliderBinaryOff.Invoke();
                sliderPosition = 0;
            }
                
        }
        prevPos = sliderPosition;

    
    
    }

    public Vector3 FindNearestPointOnLine(Vector3 origin, Vector3 end, Vector3 point)
    {
        //Get heading
        Vector3 heading = (end - origin);
        float magnitudeMax = heading.magnitude;
        heading.Normalize();

        //Do projection from the point but clamp it
        Vector3 lhs = point - origin;
        float dotP = Vector3.Dot(lhs, heading);
        dotP = Mathf.Clamp(dotP, 0f, magnitudeMax);
        return origin + heading * dotP;
    }


}
