using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetLocalTransformValues : MonoBehaviour
{
    bool m_isGrabbed;
    public void OnGrabbed(){
        m_isGrabbed = true;
        ResetLocalPositionAndRotation();
    }

    public void OnUngrabbed(){
        m_isGrabbed = false;
    }

    public void ResetLocalPositionAndRotation(){
        transform.parent = transform.parent.gameObject.GetComponent<Valve.VR.InteractionSystem.Hand>().objectAttachmentPoint;
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        /*
        foreach(Transform child in transform){
            child.localPosition = Vector3.zero;
            child.localRotation = Quaternion.identity;
            Transform child2 = child.GetChild(0);
            child2.gameObject.SetActive(false);
            StartCoroutine(doAfterFrames(1, ()=>{child2.gameObject.SetActive(true);}));
            Rigidbody rb = child.GetComponent<Rigidbody>();
            if(rb!=null){
                rb.isKinematic = true;
                StartCoroutine(doAfterFrames(2, ()=>{rb.isKinematic = false;}));
            }
           
        }*/
    }

    IEnumerator doAfterFrames(int frames, System.Action act){
        for(int i = 0; i<frames; i++)
            yield return new WaitForEndOfFrame();
        act();
    }

    void Update(){
        if(m_isGrabbed){
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
        }
    }

}
