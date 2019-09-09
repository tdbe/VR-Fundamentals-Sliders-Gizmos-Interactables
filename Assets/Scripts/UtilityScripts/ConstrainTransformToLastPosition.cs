using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Valve.VR.InteractionSystem.Sample.LockToPoint))]

public class ConstrainTransformToLastPosition : MonoBehaviour
{
    Vector3 m_returnToPos;
    Quaternion m_returnToRot;
    //Rigidbody m_rb;
    
    Valve.VR.InteractionSystem.Sample.LockToPoint m_lockToPoint;

    // Start is called before the first frame update
    void Start()
    {
        m_lockToPoint = GetComponent<Valve.VR.InteractionSystem.Sample.LockToPoint>();
        //m_rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        //positionLF = transform.position;
    }

    public void SetReturnToPoint(){
        m_returnToPos = transform.localPosition;
        m_returnToRot = transform.localRotation;
        //m_lockToPoint.enabled = true;
        m_lockToPoint.originalPosRelative = m_returnToPos;
        m_lockToPoint.originalRotRelative = m_returnToRot;
        m_lockToPoint.enabled = true;
        Debug.Log("___________________________________________________  On Trigger Exit");
    }

    public void LockToSetEnabled(bool boo){
        m_lockToPoint.enabled = boo;
        Debug.Log("___________________________________________________  On Trigger Enter");

    }

    public void ProcessObjectOutOfBoundsThisFrame(){
        //m_rb.constraints = RigidbodyConstraints.FreezeAll;
        //transform.position = m_returnToPos;
        
    }
}
