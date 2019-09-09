using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Valve.VR;

[RequireComponent(typeof(Valve.VR.InteractionSystem.Interactable))]
[RequireComponent(typeof(Valve.VR.InteractionSystem.InteractableHoverEvents))]
public class ItemHoverPlayerWhileGrabbed :MonoBehaviour{//}: MonoBehaviourSingleton<ItemHoverPlayerWhileGrabbed>

    Valve.VR.InteractionSystem.Interactable m_interactable;
    Valve.VR.InteractionSystem.InteractableHoverEvents m_interactableHE;

    // [System.Serializable]
    // public class UnityHandEvent : UnityEvent<Valve.VR.InteractionSystem.Hand>{
    // }
    // public UnityHandEvent OnHoverWhileBeingGrabbed;
    public UnityEvent OnHoverWhileBeingGrabbed_LH;
    public UnityEvent OnHoverWhileBeingGrabbed_RH;


    // Start is called before the first frame update
    void OnEnable()
    {
        m_interactable = GetComponent<Valve.VR.InteractionSystem.Interactable>();
        m_interactableHE = GetComponent<Valve.VR.InteractionSystem.InteractableHoverEvents>();
    }

    void OnHandHoverBegin(){
        if(m_interactable.attachedToHand != null && m_interactable.hoveringHand != m_interactable.attachedToHand){
            // if(OnHoverWhileBeingGrabbed!=null){
            //     OnHoverWhileBeingGrabbed.Invoke(m_interactable.hoveringHand);//
            // }

            if(m_interactable.hoveringHand.handType == SteamVR_Input_Sources.RightHand &&
             OnHoverWhileBeingGrabbed_RH!=null){
                OnHoverWhileBeingGrabbed_RH.Invoke();//
            }
            else if(m_interactable.hoveringHand.handType == SteamVR_Input_Sources.LeftHand &&
             OnHoverWhileBeingGrabbed_LH!=null){
                OnHoverWhileBeingGrabbed_LH.Invoke();//
            }

        }
    }
}
