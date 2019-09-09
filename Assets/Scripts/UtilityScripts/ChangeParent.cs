using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeParent : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public ChangeParent(Transform newParent){
        transform.parent = newParent;
    }
}
