using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class DoActionOnTrigger : MonoBehaviour
{
    [Space(30)]
    [Tooltip("Has reliable event using On Trigger/Collider Stay internally. Should work for both colliders and triggers.")]
    [Header("Has reliable event using On Trigger/Collider Stay internally. Should work for both colliders and triggers.")]
    //[Space(20)]
    //public List<Rigidbody> targetObjects;
    public LayerMask collisionLayerMask = ~0;
    
    [Tooltip("Leave the array null if you want to broadcast event normally.")]
    [Header("Leave the array null if you want to broadcast event normally.")]
    public Rigidbody[] onlySendEventsToTheseRigidbodies;
    [Tooltip("Also these are not good practice as the collision already happened on the physics layer collisions. Use in edge cases only.")]
    [Header("Also these are not good practice as the collision already happened on the physics layer collisions. Use in edge cases only.")]
    public string onlyAllowThisTag = "";
    
    //public List<GameObject> objectsToToggle;
    bool m_isTriggerBreached;
    bool m_wasTriggerBreached;

    public UnityEvent OnTriggerEnterEvent;
    public UnityEvent OnTriggerExitEvent;
    public UnityEvent OnTriggerStayEvent;
    public UnityEvent OnTriggerOutsideStayEvent;
#if UNITY_EDITOR
    [SerializeField]
    bool m_DebugLog;
    bool m_DebugLog_Verbose = false;
#endif



    void Start() {
        m_wasTriggerBreached = true;
    }
    void Update()
    {
        
        if(m_isTriggerBreached==false && m_wasTriggerBreached == true){
            if(OnTriggerExitEvent != null){
                #if UNITY_EDITOR
                if(m_DebugLog)
                    Debug.Log("OnTriggerExitEvent"+gameObject.name);
                #endif
                OnTriggerExitEvent.Invoke();
            }
        }
        else if(m_isTriggerBreached==true && m_wasTriggerBreached == false){
            if(OnTriggerEnterEvent != null){
                #if UNITY_EDITOR
                if(m_DebugLog)
                    Debug.Log("OnTriggerEnterEvent"+gameObject.name);
                #endif
                OnTriggerEnterEvent.Invoke();
            }
        }

        if(m_isTriggerBreached==false && m_wasTriggerBreached == false){
            if(OnTriggerOutsideStayEvent != null){
                
                OnTriggerOutsideStayEvent.Invoke();
            }
        }
        else
        if(m_isTriggerBreached==true){
            if(OnTriggerStayEvent!=null){
                #if UNITY_EDITOR
                //if(m_DebugLog)
                //    Debug.Log("OnTriggerStayEvent "+gameObject.name);
                #endif
                OnTriggerStayEvent.Invoke();
            }
        }
        m_wasTriggerBreached = m_isTriggerBreached;
    }

    void FixedUpdate()
    {
        
        m_isTriggerBreached = false;
    }

    bool IsRBAllowed(Rigidbody other){
        if(onlyAllowThisTag!=""){
            if(other != null && other.gameObject!=null && other.gameObject.CompareTag(onlyAllowThisTag)){
                return true;
            }
            else return false;
        }

        if(onlySendEventsToTheseRigidbodies == null || onlySendEventsToTheseRigidbodies.Length == 0)
            return true;

        foreach(Rigidbody rb in onlySendEventsToTheseRigidbodies){
            // if(onlyAllowThisTag!=""){
            //     if(other.gameObject.tag == onlyAllowThisTag && other == rb){
            //         return true;
            //     }
            // }
            // else
            if(other == rb)
                return true;
        }
        return false;
    }

    private void OnCollisionEnter(Collision other) {
        if(collisionLayerMask != ((1 << other.gameObject.layer) | collisionLayerMask))
            return;
        if(IsRBAllowed(other.rigidbody))
            m_isTriggerBreached = true;
    }

    private void OnCollisionStay(Collision other) {
        if(collisionLayerMask != ((1 << other.gameObject.layer) | collisionLayerMask))
            return;

        if(IsRBAllowed(other.rigidbody))
            m_isTriggerBreached = true;
    }

    private void OnTriggerEnter(Collider other) {
        if(collisionLayerMask != ((1 << other.gameObject.layer) | collisionLayerMask))
            return;

        if(IsRBAllowed(other.attachedRigidbody))
            m_isTriggerBreached = true;
    }

    
    private void OnTriggerStay(Collider other) {
        #if UNITY_EDITOR
        if(m_DebugLog_Verbose)
            Debug.Log("OnTriggerStay Called on: "+gameObject.transform.parent.parent.gameObject.name+"/"+gameObject.transform.parent.gameObject.name+"/"+gameObject.name+"; other: "+other.transform.parent.parent.gameObject.name+"/"+other.transform.parent.gameObject.name+"/"+other.gameObject.name);
        #endif
        //Debug.Log("On trigger stay: "+gameObject.name);
        if(collisionLayerMask != ((1 << other.gameObject.layer) | collisionLayerMask))
            return;
        
        // foreach(Rigidbody rb in targetObjects){

        //     if(other.attachedRigidbody == rb){
        //         //Debug.Log("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ m_isTriggerBreached true");

        //         m_isTriggerBreached = true;
        //         break;
        //     }
        // }

        if(IsRBAllowed(other.attachedRigidbody))
            m_isTriggerBreached = true;
    }
    
}
