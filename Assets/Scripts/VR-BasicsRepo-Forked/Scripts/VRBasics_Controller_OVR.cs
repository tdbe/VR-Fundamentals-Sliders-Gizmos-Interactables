using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRBasics_Controller_OVR : VRBasics_Controller {

	//define in the Unity editor which controller this is
	public OVRInput.Controller controller;

	void Update()
	{
		switch(VRBasics.Instance.vrType.ToString()){
		case "OVR":

			//TRIGGER
			if (OVRInput.GetUp (OVRInput.Button.PrimaryIndexTrigger, controller)) {
				SetisTrigger(false);
				SetisTriggerGrab(false);
			}
			if (OVRInput.GetDown (OVRInput.Button.PrimaryIndexTrigger, controller)) {
				SetisTrigger(true);
			}

			//GRIP
			if (OVRInput.GetUp (OVRInput.Button.PrimaryHandTrigger, controller)) {
				SetisGrip(false);
				SetisGripGrab(false);
			}
			if (OVRInput.GetDown (OVRInput.Button.PrimaryHandTrigger, controller)) {
				SetisGrip(true);
			}

			//TOUCHPAD
			if (OVRInput.GetUp (OVRInput.Button.PrimaryThumbstick, controller)) {
				SetisTouchPad(false);
				SetisTouchPadGrab(false);
			}
			if (OVRInput.GetDown (OVRInput.Button.PrimaryThumbstick, controller)) {
				SetisTouchPad(true);
			}
			break;
		
		}
	}

	public Transform GetOrigin_OVR(){
		Transform origin = this.gameObject.transform.parent.transform;
		return origin;
	}

	public Vector3 GetVelocity_OVR(){
		return OVRInput.GetLocalControllerVelocity (controller);
	}

	public Vector3 GetAngularVelocity_OVR(){
		return OVRInput.GetLocalControllerAngularVelocity (controller);
	}

	public void HapticPulse_OVR(ushort microSeconds){
		OVRInput.SetControllerVibration (microSeconds, 1.0f, controller);
	}
}
