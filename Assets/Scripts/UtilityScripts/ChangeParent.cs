using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeParent : MonoBehaviour 
{
    [SerializeField]
    Transform m_defaultParent;
    [SerializeField]
    bool m_changeOnStart;
    // Start is called before the first frame update
    void Start()
    {
        if(m_changeOnStart)
            ChangeParentToDefault();
    }

    public void ChangeParentTo(Transform newParent){
        transform.parent = newParent;
    }

    public void ChangeParentToDefault(){
        transform.parent = m_defaultParent;
    }
}
