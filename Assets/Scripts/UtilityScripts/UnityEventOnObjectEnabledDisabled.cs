using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UnityEventOnObjectEnabledDisabled : MonoBehaviour
{
    public GameObject gameObjectToTrack;
    bool m_isSelf;
    public UnityEvent OnEnableEvent;
    public UnityEvent OnDisableEvent;

    bool m_wasActive;

    void OnEnable() {
        Init();
        if(m_isSelf)
            if(OnEnableEvent!=null){
                OnEnableEvent.Invoke();
            }
    }

    void OnDisable(){
        if(m_isSelf)
            if(OnDisableEvent!=null)
                OnDisableEvent.Invoke();
    }

    // Start is called before the first frame update
    void Init()
    {
        if(gameObjectToTrack==null){
            gameObjectToTrack = gameObject;   
            m_isSelf = true;
        }
        else if(gameObjectToTrack == gameObject){
            m_isSelf = true;
        }
        
        m_wasActive = gameObjectToTrack.activeInHierarchy;

        UpdateEvents();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateEvents();
    }

    void UpdateEvents(){
        if(!m_isSelf){
            if(m_wasActive!= gameObjectToTrack.activeInHierarchy){
                if(gameObjectToTrack.activeInHierarchy)
                    if(OnEnableEvent!=null)
                        OnEnableEvent.Invoke();
                else
                    if(OnDisableEvent!=null)
                        OnDisableEvent.Invoke();
            }
            m_wasActive = gameObjectToTrack.activeInHierarchy;
        }
    }
}
