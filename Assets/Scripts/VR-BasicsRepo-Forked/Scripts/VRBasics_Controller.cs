
//======================= VRBasics_Controller ================================
//
// attach to the controller game objects
// this script is the go between for the VR controllers (depending on type) and the VRBasics scripts
// the type is held in the vrType property of VRBasics instance
//
// currently the only type available is SteamVR for the Vive
// will update as other controllers for VR devices are released
//
//=========================== by Zac Zidik ====================================

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//which buttons on the controller are used to grab objects
public enum GrabButtons
{
	grip,touchpad,trigger
}

public class VRBasics_Controller : MonoBehaviour {

	//state of the buttons on the controller
	private bool isGrip = false;
	private bool isHairTrigger = false;
	private bool isTouchPad = false;
	private bool isTrigger = false;

	//secondary state of the buttons
	//used primarily for grabbing objects
	//when a joint is broken between a grabbed object and the controller by force
	//the grab button may still be down
	//but we don't want to be able to grab things with a closed grab
	//gets set to true by the VRBascis_GrabManager when an object is grabbed using this button
	//set back to false when that button is released
	private bool isGripGrab = false;
	private bool isHairTriggerGrab = false;
	private bool isTouchPadGrab = false;
	private bool isTriggerGrab = false;

	//is the controller model being rendered
	private bool isHidden = false;

	//the x and y values of the thumb on the touchpad
	private Vector2 touchPadPos = new Vector2(0,0);

	//a hand object controlled by VRBasics_HandModel class
	public VRBasics_HandModel handModel;

	public Vector2 GetTouchPadPos(){
		return touchPadPos;
	}

	public bool GetisGrip(){
		return isGrip;
	}

	public bool GetisHairTrigger(){
		return isHairTrigger;
	}

	public bool GetisTouchPad(){
		return isTouchPad;
	}

	public bool GetisTrigger(){
		return isTrigger;
	}
		
	public void SetisGrip(bool g){
		isGrip = g;
	}

	public void SetisHairTrigger(bool ht){
		isHairTrigger = ht;
	}

	public void SetisTouchPad(bool tp){
		isTouchPad = tp;
	}

	public void SetisTrigger(bool t){
		isTrigger = t;
	}

	public bool GetisGripGrab(){
		return isGripGrab;
	}

	public void SetisGripGrab(bool g){
		isGripGrab = g;
	}

	public bool GetisHairTriggerGrab(){
		return isHairTriggerGrab;
	}

	public void SetisHairTriggerGrab(bool g){
		isHairTriggerGrab = g;
	}

	public bool GetisTouchPadGrab(){
		return isTouchPadGrab;
	}

	public void SetisTouchPadGrab(bool g){
		isTouchPadGrab = g;
	}

	public bool GetisTriggerGrab(){
		return isTriggerGrab;
	}

	public void SetisTriggerGrab(bool g){
		isTriggerGrab = g;
	}

	public Vector2 GeTtouchPadPos(){
		return touchPadPos;
	}

	public void SetTouchPadPos(Vector2 tpp){
		touchPadPos = tpp;
	}

	public bool GetIsHidden(){
		return isHidden;
	}

	public void SetIsHidden(bool h){
		isHidden = h;
	}

	public List<GameObject> GetTouchers(){
		
		List<GameObject> touchers = new List<GameObject> ();

		int numChildren = transform.childCount;
		for (int ch = 0; ch < numChildren; ch++) {
			if (transform.GetChild (ch).name == "Toucher") {
				touchers.Add (transform.GetChild (ch).gameObject);
			}
		}

		return touchers;
	}

	public List<GameObject> GetPushers(){
		
		List<GameObject> touchers = GetTouchers ();
		List<GameObject> pushers = new List<GameObject> ();

		int numTouchers = touchers.Count;
		for (int t = 0; t < numTouchers;t++) {
			int numChildren = touchers [t].transform.childCount;
			for (int ch = 0; ch < numChildren; ch++) {
				if (touchers [t].transform.GetChild (ch).name == "Pusher") {
					pushers.Add (touchers [t].transform.GetChild (ch).gameObject);
				}
			}
		}

		return pushers;
	}

	public void DisableTouchersAndPushers(bool tp){
		DisableTouchers (tp);
		DisablePushers (tp);
	}


	public void DisableTouchers(bool tp){
		//if true, no collisions should occur with the Touchers of this Controller
		if (tp) {
			
			//place all Touchers on layer where collisions are ignored
			List<GameObject> touchers = GetTouchers();
			int numTouchers = touchers.Count;
			for (int t = 0; t < numTouchers; t++) {
				touchers[t].layer = VRBasics.Instance.ignoreCollisionsLayer;
			}

		//if false
		} else if (!tp) {
			
			//place all Touchers on layer where collisions are enabled
			List<GameObject> touchers = GetTouchers();
			int numTouchers = touchers.Count;
			for (int t = 0; t < numTouchers; t++) {
				touchers[t].layer = 0;
			}
		}
	}

	public void DisablePushers(bool tp){
		
		//if true, no collisions should occur with the Pushers of this Controller
		if (tp) {
			
			//place all Pushers on layer where collisions are ignored
			List<GameObject> pushers = GetPushers();
			int numPushers = pushers.Count;
			for (int p = 0; p < numPushers; p++) {
				pushers[p].layer = VRBasics.Instance.ignoreCollisionsLayer;
			}

		//if false
		} else if (!tp) {
			
			//place all Pushers on layer where collisions are enabled
			List<GameObject> pushers = GetPushers();
			int numPushers = pushers.Count;
			for (int p = 0; p < numPushers; p++) {
				pushers[p].layer = 0;
			}
		}
	}

	public void CollideWithTouchers(GameObject other){

		List<GameObject> touchers = GetTouchers();
		int numTouchers = touchers.Count;
		for (int t = 0; t < numTouchers; t++) {
			Physics.IgnoreCollision (touchers [t].GetComponent<Collider> (), other.GetComponent<Collider> (), false);
		}
	}

	public void IgnoreTouchers(GameObject other){

		List<GameObject> touchers = GetTouchers();
		int numTouchers = touchers.Count;
		for (int t = 0; t < numTouchers; t++) {
			Physics.IgnoreCollision (touchers [t].GetComponent<Collider> (), other.GetComponent<Collider> ());
		}
	}

	public void CollideWithPushers(GameObject other){

		List<GameObject> pushers = GetPushers();
		int numPushers = pushers.Count;
		for (int p = 0; p < numPushers; p++) {
			Physics.IgnoreCollision (pushers [p].GetComponent<Collider> (), other.GetComponent<Collider> (), false);
		}
	}

	public void IgnorePushers(GameObject other){

		List<GameObject> pushers = GetPushers();
		int numPushers = pushers.Count;
		for (int p = 0; p < numPushers; p++) {
			Physics.IgnoreCollision (pushers [p].GetComponent<Collider> (), other.GetComponent<Collider> ());
		}
	}

	public Vector3 GetAngularVelocity(){
		Vector3 angularVelocity = Vector3.zero;
		switch (VRBasics.Instance.vrType.ToString ()) {
		case "OVR":
			if (GetComponent<VRBasics_Controller_OVR> ()) {
				angularVelocity = GetComponent<VRBasics_Controller_OVR> ().GetAngularVelocity_OVR ();
			}
			break;
		case "SteamVR":
			if (GetComponent<VRBasics_Controller_SteamVR> ()) {
				angularVelocity = GetComponent<VRBasics_Controller_SteamVR> ().GetAngularVelocity_SteamVR ();
			}
			break;
		}
		return angularVelocity;
	}
		
	public Transform GetOrigin(){
		Transform origin = null;
		switch (VRBasics.Instance.vrType.ToString ()) {
		case "OVR":
			if (GetComponent<VRBasics_Controller_OVR> ()) {
				origin = GetComponent<VRBasics_Controller_OVR> ().GetOrigin_OVR ();
			}
			break;
		case "SteamVR":/*
			if (GetComponent<Valve.VR.SteamVR_TrackedObject> ()) {
				origin = GetComponent<VRBasics_Controller_SteamVR> ().GetOrigin_SteamVR ();
			}*/
			break;
		}
		return origin;
	}

	public Vector3 GetVelocity(){
		Vector3 velocity = Vector3.zero;
		switch (VRBasics.Instance.vrType.ToString ()) {
		case "OVR":
			if (GetComponent<VRBasics_Controller_OVR> ()) {
				velocity = GetComponent<VRBasics_Controller_OVR> ().GetVelocity_OVR ();
			}
			break;
		case "SteamVR":
			if (GetComponent<VRBasics_Controller_SteamVR> ()) {
				velocity = GetComponent<VRBasics_Controller_SteamVR> ().GetVelocity_SteamVR ();
			}
			break;
		}
		return velocity;
	}

	public void HapticPulse(ushort microSeconds){
		switch (VRBasics.Instance.vrType.ToString ()) {
		case "OVR":
			if (GetComponent<VRBasics_Controller_OVR> ()) {
				GetComponent<VRBasics_Controller_OVR> ().HapticPulse_OVR (microSeconds);
			}
			break;
		case "SteamVR":
			if (GetComponent<VRBasics_Controller_SteamVR> ()) {
				GetComponent<VRBasics_Controller_SteamVR> ().HapticPulse_SteamVR (microSeconds);
			}
			break;
		}
	}

	public void HideController(){
		Renderer[] renderers = GetComponentsInChildren<Renderer> ();
		for (int i = 0; i < renderers.Length; i++) {
			renderers [i].enabled = false;
		}

		SetIsHidden (true);
	}

	public void ShowController(){
		Renderer[] renderers = GetComponentsInChildren<Renderer> ();
		for (int i = 0; i < renderers.Length; i++) {
			renderers [i].enabled = true;
		}

		SetIsHidden (false);
	}

	//for steamvr only, since controllers use indexes to identify themselves
	public int GetDeviceIndex(){
		int deviceIndex = -1;
		switch (VRBasics.Instance.vrType.ToString ()) {
		case "SteamVR":
			deviceIndex = GetComponent<VRBasics_Controller_SteamVR> ().GetDeviceIndex_SteamVR ();
			break;
		}
		return deviceIndex;
	}
}