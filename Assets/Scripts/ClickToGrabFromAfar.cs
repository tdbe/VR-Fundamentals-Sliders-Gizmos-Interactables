using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

 /// <summary>
/// This needs to be really high in the script execution order
/// </summary>
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
    bool _canAttach = true;
    public bool canAttach {get{return _canAttach;}}

    float m_lastAttachedTime=-1;
    
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

   public void SetCanAttach(bool val){
       _canAttach = val;
   }



    // Update is called once per frame
    void Update()
    {

        // if(OculusInputManager.Instance.GetGrabAnyClicked(OculusInputManager.Instance.GetOculusHand(trackedHands[0].handType))
        // ||
        // OculusInputManager.Instance.GetGrabAnyClicked(OculusInputManager.Instance.GetOculusHand(trackedHands[1].handType)))
        // for(int i =0; i< trackedHands.Length; i++){
        //     for(int x = 0; x< trackedHands[i].AttachedObjects.Count; x++){
        //         Debug.Log("[clicktograbfromafar "+gameObject.name+"][hand: "+trackedHands[i].name+"] trackedHands[i].AttachedObjects[x]: "+trackedHands[i].AttachedObjects[x].attachedObject.name);
        //     }

        // }

        if( canAttach )
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
                    
                    //Debug.Log("? AutoAttachToHand "+trackedHands[i].gameObject.name+"; "+gameObject.name);

                    if(ok ){//&& Time.time - m_lastAttachedTime > 2){
                        //if(m_lastAttachedTime !=-1){
                            m_autoAttach.AutoAttachToHand(trackedHands[i]);
                            Debug.Log("AutoAttachToHand "+trackedHands[i].gameObject.name+"; "+gameObject.name);
                        //}
                        //m_lastAttachedTime = Time.time;
                    }
                    break;        
                }
            }
    }
}
