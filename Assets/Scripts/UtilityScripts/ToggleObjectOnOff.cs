using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleObjectOnOff : MonoBehaviour
{
    [SerializeField]
    GameObject m_objectToToggle;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ToggleObject(){
        m_objectToToggle.SetActive(!m_objectToToggle.activeInHierarchy);
    }

    public void SetObjectActive(bool active){
        m_objectToToggle.SetActive(active);
    }
}
