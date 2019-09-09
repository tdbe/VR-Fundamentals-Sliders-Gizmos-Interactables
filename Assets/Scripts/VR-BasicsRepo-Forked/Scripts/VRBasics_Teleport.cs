
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRBasics_Teleport : MonoBehaviour {

	public VRBasics_CameraRig cameraRig;
	private GameObject camOriginalParent;
	public Transform camera;
	public float maxTeleportDistance = 10.0f;
	public float minTeleportDistance = 3.0f;
	public Transform pointer;
	public Transform reticle;
	public bool drawLine = true;
	public bool instantTeleport = true;
	public float teleportSpeed = 1.0f;
	public bool useFade = true;

	private bool onTeleportSpot = false;
	private Vector3 teleportTo;
	//while searching for a place to teleport, the gameobejct of the hit
	public GameObject teleportToGO_hit;
	//after a teleport takes place, stores the last game object of the teleport location
	public GameObject teleportToGO_last;
	private bool teleportButtonActive = false;
	private float rayLengthOut = 5.0f;
	private Quaternion reticleDefaultRotation;
	private List<GameObject> controllers;
	//a sound to play on teleport
	public AudioSource teleportAudio;
	//a sound to play while searching for teleport location
	public AudioSource teleportSearchAudio;

	void Awake(){
		//when using a reticle
		if(reticle){
			//store the initial rotation of the reticle object
			reticleDefaultRotation = reticle.localRotation;
		}
	}

	public virtual void Start(){

		// initialize line renderer component
		LineRenderer lineRenderer = GetComponent<LineRenderer>();
		if (null == lineRenderer)
		{
			LineRenderer lr = gameObject.AddComponent<LineRenderer>();
			//lr.SetWidth (0.025f, 0.01f);
		}

		//get a list of controllers in the game
		controllers = VRBasics.Instance.GetAllControllers();

		if (cameraRig.transform.parent != null) {
			camOriginalParent = cameraRig.transform.parent.gameObject;
		}
	}

	void DrawLine(Vector3[] wayPoints, RaycastHit hit){

		LineRenderer lineRenderer = GetComponent<LineRenderer> ();
		lineRenderer.enabled = true;

		Vector3[] points = MakeSmoothCurve(wayPoints, 10f);
		int numPoints = points.Length;

		lineRenderer.positionCount = numPoints;

		int vertexCount = 0;
		for(int j = 0; j < numPoints; j++){
			lineRenderer.SetPosition (vertexCount, points[j]);
			vertexCount ++;
		}
	}

	void ClearLine(){
		LineRenderer lineRenderer = GetComponent<LineRenderer> ();
		lineRenderer.enabled = false;
	}


	public static float GetPitch(Quaternion qrot)
	{
		//calculate pitch from quaternion rotation
		float pitch = Mathf.Atan2(2 * qrot.x * qrot.w - 2 * qrot.y * qrot.z, 1 - 2 * qrot.x * qrot.x - 2 * qrot.z * qrot.z);
		//change from radians to degrees between -180 and 180
		pitch *= 180/Mathf.PI;

		return pitch;
	}

	public void FoundTeleportLocation(Vector3[] wayPoints, RaycastHit hit){

		onTeleportSpot = true;
		//vector3 location to teleport to
		teleportTo = hit.point;
		//game object of the telport location found
		teleportToGO_hit = hit.transform.gameObject;


		//if drawing a line for the teleport
		if (drawLine) {
			if (teleportButtonActive) {
				DrawLine (wayPoints, hit);
			} else {
				ClearLine ();
			}
		}

		//if using a reticle on the teleport to spot
		if (reticle) {

			//set the position of the reticle to the hit
			SetReticlePosition (hit);

			if (teleportButtonActive) {

				//show the reticle
				reticle.gameObject.GetComponent<Renderer> ().enabled = true;

			} else {

				//hide the reticle
				reticle.gameObject.GetComponent<Renderer> ().enabled = false;
			}
		}
	}

	public void LostTeleportLocation(){

		//not on a spot that can be teleported to
		onTeleportSpot = false;

		//if drawing a line for the teleport
		if (drawLine) {
			ClearLine ();
		}

		//when using a reticle
		if (reticle) {

			//hide the reticle
			reticle.gameObject.GetComponent<Renderer> ().enabled = false;
		}

		teleportToGO_hit = null;
	}

	void SeachForTeleport(){

		float pitchOfPointer = GetPitch (pointer.rotation);
		//at -45 pitch, the arc should cast furthest beyond the rayLengthOut
		float percFallOff = 0.5f;

		//if controller is pointer up
		if (pitchOfPointer < 0.0f) {

			if (Mathf.Abs (pitchOfPointer) < 45.0f) {

				float distMinMax = maxTeleportDistance - minTeleportDistance;
				float perc = ((Mathf.Abs (pitchOfPointer) * 100) / 45) / 100;
				rayLengthOut = minTeleportDistance + (distMinMax * perc);
				percFallOff = 0.5f * perc;

			} else {

				float distMinMax = maxTeleportDistance - minTeleportDistance;
				float perc = (((90 - Mathf.Abs (pitchOfPointer)) * 100) / 45) / 100;
				rayLengthOut = minTeleportDistance + (distMinMax * perc);
				percFallOff = 0.5f * perc;
			}

			//if controller is pointed down
		} else {
			rayLengthOut = minTeleportDistance;
			percFallOff = 0.0f;
		}


		List<Vector3> wayPoints = new List<Vector3>();

		//first point is where the controller is
		wayPoints.Add(pointer.position);
		//second point is projected foward a distance from the controller
		wayPoints.Add(pointer.position + (pointer.forward.normalized * rayLengthOut));

		// Create a ray that points forwards from the controller
		Ray ray = new Ray(pointer.position, pointer.forward);
		RaycastHit hit;

		if (Physics.Raycast (ray, out hit, rayLengthOut)) {

			//if hit a valid teleport location with in the rayLengthOut
			if (hit.collider.GetComponent<VRBasics_TeleportLocation> () && hit.collider.GetComponent<VRBasics_TeleportLocation> ().active) {

				FoundTeleportLocation (wayPoints.ToArray (), hit);

			//if not hit with in the ray length out
			} else {

				LostTeleportLocation ();
			}
		} else {

			//create a ray that starts a certain distance from the direction the controller is pointing, included some falloff distance (depending on the pitch),and drop straight down from there
			ray = new Ray (pointer.position + (pointer.forward.normalized * rayLengthOut) + (pointer.forward.normalized * (rayLengthOut * percFallOff)), Vector3.down);

			//raycast and check for hit
			if (Physics.Raycast (ray, out hit, Mathf.Infinity)) {

				//if hit a valid teleport location
				if (hit.collider.GetComponent<VRBasics_TeleportLocation> () && hit.collider.GetComponent<VRBasics_TeleportLocation> ().active) {

					//add a third point to the way points somewhere along the ground that can be teleported to
					wayPoints.Add (hit.point);

					FoundTeleportLocation (wayPoints.ToArray (), hit);

				//if not hiting a valid teleport location
				} else {

					LostTeleportLocation ();
				}
			} else {
				LostTeleportLocation ();
			}
		}
	}

	//sets the position of the reticle when there is a hit by the raycast of the gaze
	void SetReticlePosition (RaycastHit hit){

		//put the reticle at the hit point of the raycast
		reticle.position = hit.point;

		//rotates the reticle to lay along the normal of the hit of the raycast
		reticle.rotation = Quaternion.FromToRotation (Vector3.forward, hit.normal);	
	}

	public virtual void Teleport(){

		//return camRig to original parent
		if (camOriginalParent == null){
			if (cameraRig.transform.parent != null) {
				cameraRig.transform.parent = null;
			}
		} else {
			if (cameraRig.transform.parent != camOriginalParent.transform) {
				cameraRig.transform.parent = camOriginalParent.transform;
			}
		}

		if (useFade) {
			//turn screen black
			cameraRig.SetToOut();
		}

		//move the camera rig to the position of the teleport
		Vector3 teleportLoc = teleportTo;

		//adjust to allow for the offset of the camera inside the camera rig
		teleportLoc.x = teleportTo.x - (camera.position.x - cameraRig.transform.position.x);
		teleportLoc.z = teleportTo.z - (camera.position.z - cameraRig.transform.position.z);

		if (instantTeleport) {
			//the user is immediately placed in the teleport location
			cameraRig.transform.position = teleportLoc;
		} else{
			//the user is moved over time to the teleport location
			cameraRig.StartTeleport (teleportLoc, teleportSpeed);
		}

		//game object of the last telport
		teleportToGO_last = teleportToGO_hit;

		//play the teleport sound
		if (teleportAudio) {
			teleportAudio.Play ();
		}

		if (useFade) {
			//fade back into the scene
			cameraRig.BeginFadeIn ();
		}
	}

	void Update () {

		int numControllers = controllers.Count;
		bool dPadDown = false;

		//if the pointer is a controller
		if (pointer.GetComponent<VRBasics_Controller> ()) {
			//is the D pad pressed on the pointer
			if (pointer.GetComponent<VRBasics_Controller> ().GetisTouchPad ()) {
				dPadDown = true;
			}
		} else {
			for (int c = 0; c < numControllers; c++) {
				//is the D pad pressed on any of the controllers
				if (controllers [c].GetComponent<VRBasics_Controller> ().GetisTouchPad ()) {
					dPadDown = true;
				}
			}
		}

		if (dPadDown) {

			teleportButtonActive = true;

			//play the teleport search sound
			if (teleportSearchAudio) {
				if (!teleportSearchAudio.isPlaying) {
					teleportSearchAudio.Play ();
				}
			}

		} else {

			ClearLine ();

			//if the dpad was down and was on a spot that could be teleported to
			if (teleportButtonActive && onTeleportSpot) {

				//teleport
				Teleport();
			}

			teleportButtonActive = false;

			//stop the teleport search sound
			if (teleportSearchAudio) {
				if (teleportSearchAudio.isPlaying) {
					teleportSearchAudio.Stop ();
				}
			}
		}

		//continually check for teleport 
		SeachForTeleport ();
	}

	//arrayToCurve is original Vector3 array, smoothness is the number of interpolations. 
	public static Vector3[] MakeSmoothCurve(Vector3[] arrayToCurve, float smoothness){

		List<Vector3> points;
		List<Vector3> curvedPoints;
		int pointsLength = 0;
		int curvedLength = 0;

		if(smoothness < 1.0f) smoothness = 1.0f;

		pointsLength = arrayToCurve.Length;

		curvedLength = (pointsLength*Mathf.RoundToInt(smoothness))-1;
		curvedPoints = new List<Vector3>(curvedLength);

		float t = 0.0f;
		for(int pointInTimeOnCurve = 0;pointInTimeOnCurve < curvedLength+1;pointInTimeOnCurve++){
			t = Mathf.InverseLerp(0,curvedLength,pointInTimeOnCurve);

			points = new List<Vector3>(arrayToCurve);

			for(int j = pointsLength-1; j > 0; j--){
				for (int i = 0; i < j; i++){
					points[i] = (1-t)*points[i] + t*points[i+1];
				}
			}

			curvedPoints.Add(points[0]);
		}
		return(curvedPoints.ToArray());
	}
}
