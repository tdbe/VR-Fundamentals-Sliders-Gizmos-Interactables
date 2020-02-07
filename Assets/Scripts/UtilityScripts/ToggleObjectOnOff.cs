using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleObjectOnOff : MonoBehaviour
{
    [SerializeField]
    GameObject m_objectToToggle;
    [SerializeField]
    bool m_toggleOnStart = false;
    [SerializeField]
    bool m_setSpecificValue = false;
    [SerializeField]
    bool m_specificValue =true;

    void Awake(){
        if(m_objectToToggle==null)
            m_objectToToggle = gameObject;
    }

    // Start is called before the first frame update
    void Start()
    {
        if(m_toggleOnStart){
            if(m_setSpecificValue)
                ToggleObject(m_specificValue);
            else
                ToggleObject();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ToggleObject(bool spc){
        m_objectToToggle.SetActive(spc);
        //Debug.Log(m_objectToToggle.name +" specifically enabled:  "+spc);
    }

    public void ToggleObject(){
        //m_objectToToggle.SetActive(!m_objectToToggle.activeInHierarchy);
        m_objectToToggle.SetActive(!m_objectToToggle.activeSelf);
        //Debug.Log(m_objectToToggle.name +" enabled:  "+m_objectToToggle.activeSelf);
    }

    public void SetObjectActive(bool active){
        m_objectToToggle.SetActive(active);
    }
}
