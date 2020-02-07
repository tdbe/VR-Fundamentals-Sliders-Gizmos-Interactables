#define USE_OCULUS
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;
/// <summary>
/// Magnet behaviour for hands on this object.
/// This script gets events and data from the required components.
/// It also takes a trigger action (not on trigger down, or on trigger up, but on trigger (continuous)). Use the InteractUI action or similar.
/// It is used if you want to, while your trigger is pressed down (and perhaps already having something grabbed), hover over another object (this object) and also instantly grab it, then another one etc. Like a magnet.
/// 
/// --===NOTE===--
///     you must edit the SteamVR "Throwable.cs" like this:
///         public class Throwable : MonoBehaviour
///	        {
///               public bool hoverLockIfGrabbed = true; //--- add this and turn it to false in the inspector for all objects that must behave like magnets. 
///               //An object with hoverLockIfGrabbed == true, if is in your hand, it will prevent other objects from being hovered and attached to your same hand.
///         ...
///         
///         //then in protected virtual void OnAttachedToHand( Hand hand ):
///         // replace hand.HoverLock( null ); with:
///         
///         if (!hoverLockIfGrabbed)
///                hand.HoverUnlock(null);
///            else
///                hand.HoverLock( null );
///
/// 
/// </summary>

[RequireComponent(typeof(Valve.VR.InteractionSystem.Interactable))]
[RequireComponent(typeof(Valve.VR.InteractionSystem.InteractableHoverEvents))]
[RequireComponent(typeof(Valve.VR.InteractionSystem.Throwable))]
public class AutoGrabInteractable : MonoBehaviour
{

    [Header("NOTE: Read the <summary> in this script.")]
#if !USE_OCULUS
    public SteamVR_Action_Boolean triggerDownContinuousAction = SteamVR_Actions._default.InteractUI;
#endif
    Valve.VR.InteractionSystem.Interactable m_interactable = null;
    Valve.VR.InteractionSystem.InteractableHoverEvents m_interactableHE = null;
    Valve.VR.InteractionSystem.Throwable m_throwable = null;
    public Hand lastOwnerHand = null;

    [SerializeField]
    bool m_manualMode = true;

    public bool isBeingHovered;
    public bool isBeingGrabbed;

    public bool _turnGameObjectOnWhenForceGrabbed = false;

    //public bool m_autoGrabOnStart = false;

    void Start()
    {
        // if(m_autoGrabOnStart){
        //     m_interactable.hoveringHand = lastOwnerHand;
        //     ForceGrabObject(m_interactable);    
        // }
    }

    void GetReferences(){
        m_interactable = GetComponent<Valve.VR.InteractionSystem.Interactable>();
        m_interactableHE = GetComponent<Valve.VR.InteractionSystem.InteractableHoverEvents>();
        m_throwable = GetComponent<Valve.VR.InteractionSystem.Throwable>();
    }

    void OnEnable()
    {
        GetReferences();

        m_interactableHE.onHandHoverBegin.AddListener(OnHandHoverBegin);
        m_interactableHE.onHandHoverEnd.AddListener(OnHandHoverEnd);
        m_interactableHE.onAttachedToHand.AddListener(OnAttachedToHand);
    }
    void OnDisable()
    {
        m_interactableHE.onHandHoverBegin.RemoveListener(OnHandHoverBegin);
        m_interactableHE.onHandHoverEnd.RemoveListener(OnHandHoverEnd);
        m_interactableHE.onAttachedToHand.RemoveListener(OnAttachedToHand);
        m_throwable.ManuallyDetatchThisObject();
    }

    public void ManualGrabObject()
    {
        if(m_interactable == null){
            GetReferences();
        }

        if (m_interactable.hoveringHand != null)
            lastOwnerHand = m_interactable.hoveringHand;

        #if USE_OCULUS
        //bool triggerGrab = OculusInputManager.Instance.GetGrabPinch(OculusInputManager.Instance.GetOculusHand(lastOwnerHand.handType));//
        bool triggerGrab = OculusInputManager.Instance.GetGrabAnyClicked(OculusInputManager.Instance.GetOculusHand(lastOwnerHand.handType));//
        
        if (lastOwnerHand!=null && triggerGrab)
        #else
        if (lastOwnerHand!=null && triggerDownContinuousAction.GetState(lastOwnerHand.handType))
        #endif
        {
            ForceGrabObject();
        }
    }

    public void ManualGrabObject(Interactable interactable)
    {
        

        if (interactable.hoveringHand != null)
            lastOwnerHand = interactable.hoveringHand;
        else
            lastOwnerHand = interactable.attachedToHand;
        //m_interactable = interactable;
        //m_interactable.hoveringHand = lastOwnerHand;

        #if USE_OCULUS
        //bool triggerGrab = OculusInputManager.Instance.GetGrabPinch(OculusInputManager.Instance.GetOculusHand(lastOwnerHand.handType));//
        bool triggerGrab = OculusInputManager.Instance.GetGrabAnyClicked(OculusInputManager.Instance.GetOculusHand(lastOwnerHand.handType));//
        if (lastOwnerHand!=null && triggerGrab)
        #else
        if (lastOwnerHand!=null && triggerDownContinuousAction.GetState(lastOwnerHand.handType))
        #endif
        {
            ForceGrabObject(interactable);
        }
    }

    void ForceGrabObject(Interactable interactable)
    {
        if (interactable.hoveringHand != null)
            lastOwnerHand = interactable.hoveringHand;
        
            
        if (lastOwnerHand && !lastOwnerHand.ObjectIsAttached(this.gameObject))
        {
            //Debug.Log("Hover-trigger-grabbed " + gameObject.name);
            isBeingGrabbed = true;
            if(m_throwable == null)
                m_throwable = GetComponent<Valve.VR.InteractionSystem.Throwable>();

            interactable
            .hoveringHand
            .AttachObject(
                this.gameObject,
                GrabTypes.Pinch,
                //m_interactable.useHandObjectAttachmentPoint,
                m_throwable.attachmentFlags,
                m_throwable.attachmentOffset);
        }

        if(_turnGameObjectOnWhenForceGrabbed){
            gameObject.SetActive(true);
        }
    }

    void ForceGrabObject()
    {
        if (m_interactable.hoveringHand != null)
            lastOwnerHand = m_interactable.hoveringHand;
        if (lastOwnerHand && !lastOwnerHand.ObjectIsAttached(this.gameObject))
        {
            //Debug.Log("Hover-trigger-grabbed " + gameObject.name);
            isBeingGrabbed = true;
            m_interactable.hoveringHand.AttachObject(
                this.gameObject,
                GrabTypes.Pinch,
                //m_interactable.useHandObjectAttachmentPoint,
                m_throwable.attachmentFlags,
                m_throwable.attachmentOffset);
        }

        if(_turnGameObjectOnWhenForceGrabbed){
            gameObject.SetActive(true);
        }
    }

    public void ManualLetGoOfObject()
    {
        if(m_interactable == null){
            GetReferences();
        }

        if(m_interactable.hoveringHand != null)
            lastOwnerHand = m_interactable.hoveringHand;
        if (lastOwnerHand && lastOwnerHand.ObjectIsAttached(this.gameObject))
        {
            //Debug.Log("Hover-trigger-grabbed " + gameObject.name);

            m_interactable.hoveringHand.DetachObject(this.gameObject, m_throwable.restoreOriginalParent);
            lastOwnerHand = null;
            isBeingGrabbed = false;
        }
    }

    void Update()
    {
        /*
        if (lastOwnerHand !=null && !triggerDownContinuousAction.GetState(m_interactable.hoveringHand.handType))
        {
            ManualLetGoOfObject();//TODO: not necessary?
        }*/
    }

    void OnHandHoverBegin()
    {
        isBeingHovered = true;
        if(m_interactable==null || m_interactable.hoveringHand==null)
            return;
        lastOwnerHand = m_interactable.hoveringHand;
        if (m_manualMode)
            return;

        //Debug.Log("Hover hand begin");
        if (m_interactable.isHovering)
        {
#if USE_OCULUS
            bool triggerGrab = OculusInputManager.Instance.GetGrabPinch(OculusInputManager.Instance.GetOculusHand(m_interactable.hoveringHand.handType));

            if (triggerGrab)
#else
            if (triggerDownContinuousAction.GetState(m_interactable.hoveringHand.handType))
#endif
            {
                ForceGrabObject();
            }

        }
    }

    void OnHandHoverEnd()
    {
        isBeingHovered = false;
    }

    void OnAttachedToHand()//(Hand hand)
    {
        if(m_interactable && m_interactable.hoveringHand){
            lastOwnerHand = m_interactable.hoveringHand;
            isBeingGrabbed = true;
        }
    }

    void OnDetachedFromHand()//(Hand hand)
    {
        lastOwnerHand = null;
        isBeingGrabbed = false;

    }


}
