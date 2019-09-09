using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(GLHandleVisuals_Group))]
public class GLHandlesSteamVREventsAssigner : MonoBehaviour
{
    public string info = "This script takes hover events from InteractableHoverEvents found either in parents or children. It should be on a layer that does not interact with the controller on its own, because the interaction is handled by the InteractableHoverEvents.";
    [Header("These are usually auto-populated at runtime.")]
    [SerializeField]
    GLHandleVisuals_Group m_GLHandleVisuals_Group;
    //
    [SerializeField]
    Valve.VR.InteractionSystem.InteractableHoverEvents m_InteractableHoverEvents;
    
    Valve.VR.InteractionSystem.Interactable m_interactable;
    Valve.VR.InteractionSystem.Throwable m_throwable;
    GameObject targetGO;




    [HideInInspector]
    public Valve.VR.InteractionSystem.Interactable getInteractable{get{return m_interactable;}}    
    public Valve.VR.InteractionSystem.Throwable getThrowable{get{return m_throwable;}}    
    public GameObject getGameObject{get{return targetGO;}}    
    
    // Start is called before the first frame update
    void Awake()
    {   
        m_GLHandleVisuals_Group = GetComponent<GLHandleVisuals_Group>();
        if(m_InteractableHoverEvents==null){
            m_InteractableHoverEvents = GetComponent<Valve.VR.InteractionSystem.InteractableHoverEvents>();
            m_interactable = GetComponent<Valve.VR.InteractionSystem.Interactable>();
            m_throwable = GetComponent<Valve.VR.InteractionSystem.Throwable>();
            targetGO = gameObject;
        }
        if(m_InteractableHoverEvents==null){
            Transform tmpTransform = transform;
            for(int i = 0; i< 100; i++){
                tmpTransform = tmpTransform.parent;
                if(tmpTransform!=null){
                    m_InteractableHoverEvents = tmpTransform.GetComponent<Valve.VR.InteractionSystem.InteractableHoverEvents>();
                    m_interactable = tmpTransform.GetComponent<Valve.VR.InteractionSystem.Interactable>();
                    m_throwable = tmpTransform.GetComponent<Valve.VR.InteractionSystem.Throwable>();
                    targetGO = tmpTransform.gameObject;
                }
                else
                    break;
                    
                if(m_InteractableHoverEvents!=null)
                    break;
            }
            if(m_InteractableHoverEvents==null){
                m_InteractableHoverEvents = transform.GetComponentInChildren<Valve.VR.InteractionSystem.InteractableHoverEvents>();
                m_interactable = transform.GetComponent<Valve.VR.InteractionSystem.Interactable>();
                m_throwable = transform.GetComponent<Valve.VR.InteractionSystem.Throwable>();
                targetGO = transform.gameObject;
            }
            else if(m_InteractableHoverEvents==null){
                Debug.Log("["+transform.parent.name+"/"+gameObject.name+"] WARNING: Hovering on these gizmos will not work because the script couldn't auto-find the InteractableHoverEvents it should subscribe to. Assign it manually in the inspector.");
            }
        }
        
        if(m_interactable==null && m_InteractableHoverEvents!=null){
            m_interactable = m_InteractableHoverEvents.GetComponent<Valve.VR.InteractionSystem.Interactable>();
        }
        m_GLHandleVisuals_Group.interactableLinkedTo = getInteractable;   
    }

    void OnEnable(){
        #if UNITY_EDITOR
        if(m_InteractableHoverEvents==null){
            Awake();
        }
        #endif
        if(m_InteractableHoverEvents!=null){
            m_InteractableHoverEvents.onHandHoverBegin.AddListener(m_GLHandleVisuals_Group.Hovered);
            m_InteractableHoverEvents.onHandHoverEnd.AddListener(m_GLHandleVisuals_Group.UnHovered);
            m_InteractableHoverEvents.onAttachedToHand.AddListener(m_GLHandleVisuals_Group.Clicked);
            m_InteractableHoverEvents.onDetachedFromHand.AddListener(m_GLHandleVisuals_Group.UnClicked);
        }
    }

    void OnDisable(){
        #if UNITY_EDITOR
        if(m_InteractableHoverEvents==null){
            Awake();
        }
        #endif
        if(m_InteractableHoverEvents!=null){
            m_InteractableHoverEvents.onHandHoverBegin.RemoveListener(m_GLHandleVisuals_Group.Hovered);
            m_InteractableHoverEvents.onHandHoverEnd.RemoveListener(m_GLHandleVisuals_Group.UnHovered);
            m_InteractableHoverEvents.onAttachedToHand.RemoveListener(m_GLHandleVisuals_Group.Clicked);
            m_InteractableHoverEvents.onDetachedFromHand.RemoveListener(m_GLHandleVisuals_Group.UnClicked);
        }
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
