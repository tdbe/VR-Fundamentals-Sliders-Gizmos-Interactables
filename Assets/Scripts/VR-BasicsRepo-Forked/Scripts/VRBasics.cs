
//============================ VRBasics =======================================
//
// this is meant to be a Singleton class object
// it is also set to appear in every scene without reconfiguration
// in your Layer manager, you must add a layer called Ignore Collisions
// assign the index of this layer to the ignoreCollisionsLayer property of this class
// Under Edit -> Project Settings -> Physics use the Layer Collision Matrix to disable
// all collisions between the Ignore Collisions layer and all other layers
// this gives your application a layer where all collisions are ignored
// and allows some of the concepts of VRBasics to function properly
//
//=========================== by Zac Zidik ====================================


using UnityEngine;
using UnityEngine.VR;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class VRBasics : MonoBehaviour {

	//will hold a single instance of this class
	private static VRBasics _instance = null;

	public bool autoDeviceSwitch = false;
	public bool autoVRType = false;

	//returns the instance of this class
	public static VRBasics Instance { get { return _instance;}}

	//a list of possible VR types supported by VRBasics
	public enum VRTypes{SteamVR, OVR};
	//instance of the list
	public VRTypes vrType;

	//layer index number
	//check the physics manager
	public int ignoreCollisionsLayer = 8;
	public float renderScale = 1.0f;

	void OnEnable(){
		SceneManager.sceneLoaded += OnSceneLoaded;
	}

	void OnSceneLoaded(Scene scene, LoadSceneMode mode){
		//if using automatic detection of connected device
		if (autoVRType) {
			string model = UnityEngine.XR.XRDevice.model != null ? UnityEngine.XR.XRDevice.model : "";
			Debug.Log (model);

			//deactivates the uneeded camera rig
			//both must be active in the editor to use this
			if (model.IndexOf ("Rift") >= 0 || model.IndexOf ("Quest") >= 0) {
				//VRSettings.LoadDeviceByName ("Oculus");
				VRBasics.Instance.vrType = VRBasics.VRTypes.OVR;
				GameObject.Find ("SteamVRCameraRig [VRBasics]").SetActive (false);
				GameObject.Find ("OVRCameraRig [VRBasics]").SetActive (true);
			} else {
				//VRSettings.LoadDeviceByName ("OpenVR");
				VRBasics.Instance.vrType = VRBasics.VRTypes.SteamVR;
				GameObject.Find ("SteamVRCameraRig [VRBasics]").SetActive (true);
				GameObject.Find ("OVRCameraRig [VRBasics]").SetActive (false);
			}
		}
	}

	void Awake(){
		//insure this object class is a singleton
		//if there is already an instance of this class
		//and it is not this object
		if (_instance != null && _instance != this) {
			//get rid of any other instance
			Destroy (this.gameObject);
		} else {
			//make this the single instance
			_instance = this;
			//keep in all scenes
			DontDestroyOnLoad (this);
		}

		//ignore collisions between all Touchers
		List<GameObject> touchers = GetAllTouchers ();
		int numTouchers = touchers.Count;
		for (int t = 0; t < numTouchers;t++) {
			IgnoreAllTouchers (touchers [t]);
		}

		//ignore collisions between all Pushers
		List<GameObject> pushers = GetAllPushers ();
		int numPushers = pushers.Count;
		for (int p = 0; p < numPushers;p++) {
			IgnoreAllPushers (pushers [p]);
		}

		//ignore collisions between all Touchers and Pushers
		for (int t = 0; t < numTouchers;t++) {
			IgnoreAllPushers (touchers [t]);
		}

		UnityEngine.XR.XRSettings.eyeTextureResolutionScale = renderScale;
	}

	void OnDisable(){
		SceneManager.sceneLoaded -= OnSceneLoaded;
	}


	public List<GameObject> GetAllControllers(){
		List<GameObject> controllers = new List<GameObject> ();
		foreach (GameObject go in Resources.FindObjectsOfTypeAll(typeof(GameObject)) as GameObject[]) {

			if (go.GetComponent<VRBasics_Controller> ()) {
				controllers.Add (go);
			}
		}

		return controllers;
	}

	public List<GameObject> GetAllTouchers(){

		List<GameObject> controllers = GetAllControllers ();
		List<GameObject> touchers = new List<GameObject> ();

		int numControllers = controllers.Count;
		for (int c = 0; c < numControllers; c++) {
			int numChildren = controllers [c].transform.childCount;
			for (int ch = 0; ch < numChildren; ch++) {
				if (controllers [c].transform.GetChild (ch).name == "Toucher") {
					touchers.Add (controllers [c].transform.GetChild (ch).gameObject);
				}
			}
		}

		return touchers;
	}

	public List<GameObject> GetAllPushers(){
		
		List<GameObject> touchers = GetAllTouchers ();
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

	public void CollideWithAllTouchers(GameObject other){

		List<GameObject> touchers = GetAllTouchers ();
		int numTouchers = touchers.Count;
		for (int t = 0; t < numTouchers; t++) {

			Physics.IgnoreCollision (touchers [t].GetComponent<Collider> (), other.GetComponent<Collider> (), false);
		}
	}

	public void CollideWithAllPushers(GameObject other){

		List<GameObject> pushers = GetAllPushers ();
		int numPushers = pushers.Count;
		for (int p = 0; p < numPushers; p++) {

			Physics.IgnoreCollision (pushers [p].GetComponent<Collider> (), other.GetComponent<Collider> (), false);
		}
	}

	public void IgnoreAllTouchers(GameObject other){

		List<GameObject> touchers = GetAllTouchers ();
		int numTouchers = touchers.Count;
		for (int t = 0; t < numTouchers; t++) {

			Physics.IgnoreCollision (touchers [t].GetComponent<Collider> (), other.GetComponent<Collider> ());
		}
	}

	public void IgnoreAllPushers(GameObject other){

		List<GameObject> pushers = GetAllPushers ();
		int numPushers = pushers.Count;
		for (int p = 0; p < numPushers; p++) {

			Physics.IgnoreCollision (pushers [p].GetComponent<Collider> (), other.GetComponent<Collider> ());
		}
	}
}


