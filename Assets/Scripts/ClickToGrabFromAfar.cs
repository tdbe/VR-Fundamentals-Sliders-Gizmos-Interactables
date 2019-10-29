using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class ClickToGrabFromAfar : MonoBehaviour
{
    [TextArea(2,4)]
    public string info = "If the target object is not grabbed by any hand, and one hand clicks a trigger/grip, that hand will auto grab the target object. Note: You might want to enable snap on the Throwable to make the object teleport to your hand.";
    
    [Space(15)]
    [SerializeField]
    bool m_grabWhenAlreadyBeingGrabbed = false;
    [SerializeField]
    bool m_regrabEvenWhenAlreadyGrabbedBySameHand = false;

    [Space(15)]
    [Header("You must assign these, or implement something in OculusInputManager.")]
    [SerializeField]
    Hand[] trackedHands;

    [Header("Autopopulated to this.gameObject(.AutoAttachInteractable) if null.")]
    [SerializeField]
    GameObject m_targetObject;
    [SerializeField]
    AutoAttachInteractable m_autoAttach;
    
    // Start is called before the first frame update
    void Awake()
    {
        if(m_targetObject == null){
            m_targetObject = gameObject;
            Debug.Log("[ClickToGrabFromAfar] m_targetObject was null, using this.gameobject instead.");
        }
        if(m_autoAttach==null)
            m_autoAttach = m_targetObject.GetComponent<AutoAttachInteractable>();
    }

   

    // Update is called once per frame
    void Update()
    {
        for(int i =0; i< trackedHands.Length; i++){
            if(OculusInputManager.Instance.GetGrabAnyClicked(OculusInputManager.Instance.GetOculusHand(trackedHands[i].handType))){
                bool ok = true;
                for(int j =0; j< trackedHands.Length; j++){
                    if(trackedHands[j].ObjectIsAttached(m_targetObject)){
                        if(!m_grabWhenAlreadyBeingGrabbed)
                            ok = false;   
                        else if(j==i && !m_regrabEvenWhenAlreadyGrabbedBySameHand)
                            ok = false;
                    }
                }
                if(ok){
                    m_autoAttach.AutoAttachToHand(trackedHands[i]);
                }
                break;        
            }
        }
    }
}
