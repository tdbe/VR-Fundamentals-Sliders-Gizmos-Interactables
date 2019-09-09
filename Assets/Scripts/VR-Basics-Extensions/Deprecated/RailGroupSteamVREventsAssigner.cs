using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RailGroup))]
public class RailGroupSteamVREventsAssigner : MonoBehaviour
{
    [Header("These are auto-populated at runtime.")]
    [SerializeField]
    RailGroup m_RailGroup;
    [SerializeField]
    Valve.VR.InteractionSystem.InteractableHoverEvents m_InteractableHoverEvents;


    // Start is called before the first frame update
    void Awake()
    {   
        m_RailGroup = GetComponent<RailGroup>();
        m_InteractableHoverEvents = GetComponent<Valve.VR.InteractionSystem.InteractableHoverEvents>();
        if(m_InteractableHoverEvents==null){
            Transform tmpTransform = transform;
            for(int i = 0; i< 100; i++){
                tmpTransform = tmpTransform.parent;
                if(tmpTransform!=null)
                    m_InteractableHoverEvents = tmpTransform.GetComponent<Valve.VR.InteractionSystem.InteractableHoverEvents>();
                else
                    break;

                if(m_InteractableHoverEvents!=null)
                    break;
            }
            if(m_InteractableHoverEvents==null){
                m_InteractableHoverEvents = transform.GetComponentInChildren<Valve.VR.InteractionSystem.InteractableHoverEvents>();
            }
        }
        
    }

    void OnEnable(){
        // m_InteractableHoverEvents.onHandHoverBegin.AddListener(m_RailGroup.Hovered);
        // m_InteractableHoverEvents.onHandHoverEnd.AddListener(m_RailGroup.UnHovered);
        // m_InteractableHoverEvents.onAttachedToHand.AddListener(m_RailGroup.Clicked);
        // m_InteractableHoverEvents.onDetachedFromHand.AddListener(m_RailGroup.UnClicked);
    }

    void OnDisable(){
        // m_InteractableHoverEvents.onHandHoverBegin.RemoveListener(m_RailGroup.Hovered);
        // m_InteractableHoverEvents.onHandHoverEnd.RemoveListener(m_RailGroup.UnHovered);
        // m_InteractableHoverEvents.onAttachedToHand.RemoveListener(m_RailGroup.Clicked);
        // m_InteractableHoverEvents.onDetachedFromHand.RemoveListener(m_RailGroup.UnClicked);
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
