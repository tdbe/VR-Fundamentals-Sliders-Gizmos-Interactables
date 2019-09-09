using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRenderEventForwarder : MonoBehaviour
{
    public delegate void OnPostRenderEventHandler(Camera senderCamera);  
    public static event OnPostRenderEventHandler OnPostRenderEvent;  

    Camera m_cam;

    void Awake(){
        m_cam = GetComponent<Camera>();    
    }

    void OnPostRender(){
        if(OnPostRenderEvent!=null){
            OnPostRenderEvent(m_cam);
        }
    }
}
