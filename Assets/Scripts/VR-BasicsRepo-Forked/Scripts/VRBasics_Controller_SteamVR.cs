using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRBasics_Controller_SteamVR : VRBasics_Controller {

	//holds the index of the input device
	private int deviceIndex;

	void Update()
	{
		switch(VRBasics.Instance.vrType.ToString()){
		case "SteamVR":
			//constantly update the index of this device cause it might change
			deviceIndex = GetDeviceIndex ();

			//TRIGGER
			if (SteamVR_Controller.Input (deviceIndex).GetPressUp (SteamVR_Controller.ButtonMask.Trigger)) {
				SetisTrigger(false);
				SetisTriggerGrab(false);
			}
			if (SteamVR_Controller.Input (deviceIndex).GetPressDown (SteamVR_Controller.ButtonMask.Trigger)) {
				SetisTrigger(true);
			}

			//HAIRTRIGGER
			if (SteamVR_Controller.Input (deviceIndex).GetHairTrigger ()) {
				SetisHairTrigger (true);
			} else {
				SetisHairTrigger (false);
				SetisHairTriggerGrab (false);
				//this also means the tigger is not down
				SetisTrigger(false);
			}

			//GRIP
			if (SteamVR_Controller.Input (deviceIndex).GetPressUp (SteamVR_Controller.ButtonMask.Grip)) {
				SetisGrip(false);
				SetisGripGrab(false);
			}
			if (SteamVR_Controller.Input (deviceIndex).GetPressDown (SteamVR_Controller.ButtonMask.Grip)) {
				SetisGrip(true);
			}

			//TOUCHPAD
			if (SteamVR_Controller.Input (deviceIndex).GetPressUp (SteamVR_Controller.ButtonMask.Touchpad)) {
				SetisTouchPad(false);
				SetisTouchPadGrab(false);
			}
			if (SteamVR_Controller.Input (deviceIndex).GetPressDown (SteamVR_Controller.ButtonMask.Touchpad)) {
				SetisTouchPad(true);
			}


			//while the touchPad is being touched
			if (GetisTouchPad()) {
				//constantly update the index of this device cause it might change
				deviceIndex = GetDeviceIndex ();
				SetTouchPadPos(new Vector2 (SteamVR_Controller.Input (deviceIndex).GetAxis (Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad).x, SteamVR_Controller.Input (deviceIndex).GetAxis (Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad).y));
			} else {
				//zero out the touch pad when it is not being touched
				if (GetTouchPadPos().x != 0 || GetTouchPadPos().y != 0) {
					SetTouchPadPos (new Vector2 (0, 0));
				}
			}
			break;
		}
	}

	public Transform GetOrigin_SteamVR(){
		Transform origin = null;
		/*if (GetComponent<Valve.VR.SteamVR_TrackedObject> ()) {
			origin = GetComponent<Valve.VR.SteamVR_TrackedObject> ().origin ? GetComponent<Valve.VR.SteamVR_TrackedObject> ().origin : GetComponent<Valve.VR.SteamVR_TrackedObject> ().transform.parent;
		}*/
		return origin;
	}

	public Vector3 GetVelocity_SteamVR(){
		return SteamVR_Controller.Input (deviceIndex).velocity;
	}

	public Vector3 GetAngularVelocity_SteamVR(){
		return SteamVR_Controller.Input (deviceIndex).angularVelocity;
	}

	public void HapticPulse_SteamVR(ushort microSeconds){
		//SteamVR_Controller.Input (deviceIndex).TriggerHapticPulse (microSeconds);
	}

	public int GetDeviceIndex_SteamVR(){
		int deviceIndex = -1;/*
		if (GetComponent<Valve.VR.SteamVR_TrackedObject> ()) {
			deviceIndex = (int)GetComponent<Valve.VR.SteamVR_TrackedObject> ().index;
		}*/
		return deviceIndex;
	}
}
