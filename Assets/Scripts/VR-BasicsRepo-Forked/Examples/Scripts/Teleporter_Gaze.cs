using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleporter_Gaze : MonoBehaviour {

	private List<GameObject> controllers;
	private bool onTeleportSpot = false;
	private bool teleportActive = false;
	public GameObject cameraRig;

	public Transform gazeCamera;
	public float rayLength = 500f;
	public float rayDuration = 1f;
	public bool useDebugRay;
	public Transform reticle;
	public float reticleDefaultDistance = 2.0f;
	private Quaternion reticleDefaultRotation;
	public bool onlyHitGazables = false;
	public bool useNormal = false;

	//the current object under gaze
	private VRBasics_Gazable curGazedObject;
	//the previous object under gaze
	private VRBasics_Gazable prevGazedObject;

	// Use this for initialization
	void Start () {

		//get a list of controllers in the game
		controllers = VRBasics.Instance.GetAllControllers();
	}
	
	void Awake(){

		//when using a reticle
		if(reticle){

			//store the initial rotation of the reticle object
			reticleDefaultRotation = reticle.localRotation;
		}
	}

	void Gaze(){

		// Create a ray that points forwards from the camera.
		Ray ray = new Ray(gazeCamera.position, gazeCamera.forward);
		RaycastHit hit;

		//raycast and check for hit
		if (Physics.Raycast (ray, out hit, rayLength)) {

			//if hit a valid teleport location
			if (hit.collider.GetComponent<VRBasics_TeleportLocation> () && hit.collider.GetComponent<VRBasics_TeleportLocation> ().active) {

				//if gaze is only used to hit gazable objects
				if (onlyHitGazables) {

					//if the object hit by the gaze is a gazable object
					if (hit.collider.GetComponent<VRBasics_Gazable> ()) {

						//if the object hit by the raycast is a gazable object
						VRBasics_Gazable gazabledObject = hit.collider.GetComponent<VRBasics_Gazable> ();

						//assign the current object under gaze
						curGazedObject = gazabledObject;

						//if the gazable object is not the same as the previously gazed object
						//stops from continously identifying the same object
						if (gazabledObject && gazabledObject != prevGazedObject) {

							//set the gazable object to active
							gazabledObject.Activate ();
						}

						//if there was previously gazed object
						if (gazabledObject != prevGazedObject && prevGazedObject != null) {

							//deactivate it
							prevGazedObject.Deactivate ();
						}

						//assign the previous object under gaze
						prevGazedObject = gazabledObject;

						//when using a reticle
						if (reticle) {

							//set the position of the reticle to the hit
							SetReticlePosition (hit);

							if (teleportActive) {
								//show the reticle
								reticle.gameObject.GetComponent<Renderer> ().enabled = true;

								//not on a spot that can be teleported to
								onTeleportSpot = true;

							} else {
								//hide the reticle
								reticle.gameObject.GetComponent<Renderer> ().enabled = false;

								//not on a spot that can be teleported to
								onTeleportSpot = false;
							}
						}

						//if the object hit by the gaze is not a gazable object
					} else {

						//if there was previously gazed object
						if (prevGazedObject != null) {

							//deactivate it
							prevGazedObject.Deactivate ();
							prevGazedObject = null;
						}

						//clear the current gazed object as well
						curGazedObject = null;

						//when using a reticle
						if (reticle) {

							//set it to it's fault distance and position from the camera
							SetReticlePosition ();

							//hide the reticle
							reticle.gameObject.GetComponent<Renderer> ().enabled = false;

							//not on a spot that can be teleported to
							onTeleportSpot = false;
						}
					}

					//if the gaze can hit any object with a collider
				} else {

					//if the object hit by the raycast is a gazable object
					VRBasics_Gazable gazabledObject = hit.collider.GetComponent<VRBasics_Gazable> ();

					//assign the current object under gaze
					curGazedObject = gazabledObject;

					//if the gazable object is not the same as the previously gazed object
					//stops from continously identifying the same object
					if (gazabledObject && gazabledObject != prevGazedObject) {

						//set the gazable object to active
						gazabledObject.Activate ();
					}

					//if there was previously gazed object
					if (gazabledObject != prevGazedObject && prevGazedObject != null) {

						//deactivate it
						prevGazedObject.Deactivate ();
					}

					//assign the previous object under gaze
					prevGazedObject = gazabledObject;

					//when using a reticle
					if (reticle) {

						//set the position of the reticle to the hit
						SetReticlePosition (hit);
					}
				}
			
			//if not hiting a valid teleport location
			} else {

				//not on a spot that can be teleported to
				onTeleportSpot = false;

				//when using a reticle
				if (reticle) {

					//hide the reticle
					reticle.gameObject.GetComponent<Renderer> ().enabled = false;
				}
			}

		//when nothing is hit when the raycast
		} else {

			//if there was previously gazed object
			if (prevGazedObject != null){

				//deactivate it
				prevGazedObject.Deactivate ();
				prevGazedObject = null;
			}

			//clear the current gazed object as well
			curGazedObject = null;

			//not on a spot that can be teleported to
			onTeleportSpot = false;

			//hide the reticle
			reticle.gameObject.GetComponent<Renderer> ().enabled = false;
		}


		//when using a debug ray
		if (useDebugRay){

			//draw the debug ray
			Debug.DrawRay(gazeCamera.position, gazeCamera.forward * rayLength, Color.red, rayDuration);
		}
	}

	//sets the position of the reticle when there is no hit by the raycast of the gaze
	void SetReticlePosition(){

		//put the reticle in front of the camera at a set distance
		reticle.position = gazeCamera.position + gazeCamera.forward * reticleDefaultDistance;

		//keeps the reticle at its initial rotation in front of the camera
		reticle.localRotation = reticleDefaultRotation;
	}

	//sets the position of the reticle when there is a hit by the raycast of the gaze
	void SetReticlePosition (RaycastHit hit){

		//put the reticle at the hit point of the raycast
		reticle.position = hit.point;

		//when using normal detection
		if (useNormal){

			//rotates the reticle to lay along the normal of the hit of the raycast
			reticle.rotation = Quaternion.FromToRotation (Vector3.forward, hit.normal);
		}else{

			//keeps the reticle at its initial rotation in front of the camera
			reticle.localRotation = reticleDefaultRotation;
		}
	}

	void Teleport(){

		//turn screen black
		cameraRig.GetComponent<VRBasics_CameraRig> ().SetToOut ();

		//move the camera rig to the position of the reticle
		Vector3 teleportLoc = reticle.position;

		//adjust to allow for the offset of the camera inside the camera rig
		teleportLoc.x = reticle.position.x - (gazeCamera.position.x - cameraRig.transform.position.x);
		teleportLoc.z = reticle.position.z - (gazeCamera.position.z - cameraRig.transform.position.z);

		cameraRig.transform.position = teleportLoc;

		//fade back into the scene
		cameraRig.GetComponent<VRBasics_CameraRig> ().BeginFadeIn ();
	}

	void Update () {


		int numControllers = controllers.Count;
		bool dPadDown = false;
		for (int c = 0; c < numControllers; c++) {
			//is the D pad pressed on any of the controllers
			if (controllers [c].GetComponent<VRBasics_Controller> ().GetisTouchPad ()) {
				dPadDown = true;
			}
		}

		if (dPadDown) {
			
			teleportActive = true;

		} else {

			//if the dpad was down and was on a spot that could be teleported to
			if (teleportActive && onTeleportSpot) {

				//teleport
				Teleport();
			}

			teleportActive = false;
		}


		if (teleportActive) {
			//show the reticle
			reticle.gameObject.GetComponent<Renderer> ().enabled = true;
		} else {
			//hide the reticle
			reticle.gameObject.GetComponent<Renderer>().enabled = false;
		}

		//continually check for gaze 
		Gaze ();
	}
}
