using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent( typeof( Valve.VR.InteractionSystem.Interactable ) )]
public class DropCloneInPlaceWhenGrabbed : MonoBehaviour
{
    
    Vector3 startpos;
    Quaternion startrot;
    bool grabbed = false;
    
    // Start is called before the first frame update
    void Start()
    {
        startpos = transform.position;
        startrot = transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        if(!grabbed){
            startpos = transform.position;
            startrot = transform.rotation;
        }
    }

    void OnAttachedToHand(){
        grabbed = true;

        Instantiate(gameObject, startpos, startrot);
    }
}
