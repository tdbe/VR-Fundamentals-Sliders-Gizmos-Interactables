using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class KinematicToggler : MonoBehaviour
{   
    Rigidbody m_rb;
    [Header("The parameters below are optional. Main use case is to call `public void SetIsKinematic(bool val)`")]
    [SerializeField]
    int m_doAfterFrames = -1;
    [SerializeField]
    bool m_setToAfterFrames = false;
    [SerializeField]
    bool m_setEveryFrame;

    // Start is called before the first frame update
    void Awake()
    {
        m_rb = GetComponent<Rigidbody>();
        if(m_doAfterFrames>-1){
            StartCoroutine(DoActionAfterFrames(2, ()=>{m_rb.isKinematic = m_setToAfterFrames;}));
        }
    }

    void LateUpdate(){
        if(m_doAfterFrames>-1){
            if(m_setEveryFrame){
                m_rb.isKinematic = m_setToAfterFrames;
            }
        }
    }

    // Update is called once per frame
    public void SetIsKinematic(bool val)
    {
        m_rb.isKinematic = val;
    }

    IEnumerator DoActionAfterFrames(int frames, System.Action act){
        for(int i = 0; i< frames; i++)
            yield return new WaitForEndOfFrame();
        act();
    }
}
