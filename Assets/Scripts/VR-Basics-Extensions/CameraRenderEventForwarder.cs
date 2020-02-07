using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class CameraRenderEventForwarder : MonoBehaviour
{
    public delegate void OnPostRenderEventHandler(Camera senderCamera);  
    public static event OnPostRenderEventHandler OnPostRenderEvent;  

    public delegate void OnRenderEventHandler(Camera senderCamera);  
    public static event OnRenderEventHandler OnRenderEvent;  

    Camera m_cam;

    void Awake(){
        m_cam = GetComponent<Camera>();    
    }

    void OnPostRender(){
        if(OnPostRenderEvent!=null){
            OnPostRenderEvent(m_cam);
        }
    }

    void OnRenderObject() {
        if(OnRenderEvent!=null){
            OnRenderEvent(m_cam);
        }
    }
}
