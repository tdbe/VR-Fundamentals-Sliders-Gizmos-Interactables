using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class OculusInputLaserReticleTracker : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// This should be called automatically via SendMessage("OnAttachedToHand") 
    /// if this script is attached to a gameobject that has the steamvr Interactable script on it.
    /// </summary>
    /// <param name="hand"></param>
    public void OnAttachedToHand(Hand hand){
        OculusInputManager.Instance.CurrentControllerOfReticle =
        OculusInputManager.Instance.GetOculusHand(hand.handType);
    }
}
