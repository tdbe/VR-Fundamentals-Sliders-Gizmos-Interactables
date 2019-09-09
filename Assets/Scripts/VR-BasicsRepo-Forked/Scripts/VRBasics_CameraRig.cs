using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class VRBasics_CameraRig : MonoBehaviour {

	private float groundY = 0.0f;
	private float curX;
	private float curY;
	private float curZ;
	private float targetY;
	private Vector3 targetVec;
	public enum RigState{Grounded, Climbing, Falling, Floating, Teleporting, Flapping, Bump};
	public RigState rigState;

	private float start = 0.0f;
	private float distance = 0.0f;
	private float duration = 1.0f;
	private float elapsedTime = 0.0f;
	private Vector3 startVec = Vector3.zero;
	private Vector3 distanceVec = Vector3.zero;
	private float threshold = 0.01f;
	private float targAlpha = 0.0f;
	private float startAlpha = 0.0f;
	private float curAlpha = 0.0f;
	private float distAlpha = 0.0f;
	private float elapsedTimeAlpha = 0.0f;

	private Interpolate.Function climbing;
	private Interpolate.EaseType easeClimbing = Interpolate.EaseType.EaseOutQuad;

	private Interpolate.Function falling; 
	private Interpolate.EaseType easeFalling = Interpolate.EaseType.EaseInQuad;

	private Interpolate.Function teleporting;
	private Interpolate.EaseType easeTeleporting = Interpolate.EaseType.Linear;

	private Interpolate.Function bumping;
	private Interpolate.EaseType easeBumping = Interpolate.EaseType.EaseInQuad;

	private Interpolate.Function fade;
	private Interpolate.EaseType easefade = Interpolate.EaseType.Linear; 

	private GameObject originalParent;

	public Camera cam;
	public GameObject Panel_Fade;
	public float fadeSpeed = 1.0f;
	private bool fadingIn = false;
	private bool fadingOut = false;

	void Awake(){
		climbing = Interpolate.Ease(easeClimbing);
		falling = Interpolate.Ease(easeFalling);
		teleporting = Interpolate.Ease(easeTeleporting);
		bumping = Interpolate.Ease(easeBumping);
		fade = Interpolate.Ease(easefade);
	}

	public float GetGround(){
		return groundY;
	}

	void Start () {
		groundY = transform.position.y;
		curY = transform.position.y;
		rigState = RigState.Grounded;
		if (transform.parent != null) {
			originalParent = transform.parent.gameObject;
		}
	}

	public GameObject GetOriginalParent(){
		return originalParent;
	}

	public void Reparent(GameObject newParent){
		transform.parent = newParent.transform;
	}

	public void ReturnToOrginalParent(){
		if (originalParent == null){
			if (transform.parent != null) {
				transform.parent = null;
			}
		} else {
			if (transform.parent != originalParent.transform) {
				transform.parent = originalParent.transform;
			}
		}
	}

	public void StartClimb(float targY, float dur){
		targetY = targY;
		start = transform.position.y;
		distance = targetY - transform.position.y;
		duration = dur;
		elapsedTime = 0.0f;
		rigState = RigState.Climbing;
	}

	public void StartFall(float targY, float dur){
		targetY = targY;
		start = transform.position.y;
		distance = targetY - transform.position.y;
		duration = dur;
		elapsedTime = 0.0f;
		rigState = RigState.Falling;
	}

	public void StartTeleport(Vector3 targVec, float dur){
		targetVec = targVec;
		startVec = transform.position;
		distanceVec = new Vector3 (targetVec.x - transform.position.x, targetVec.y - transform.position.y, targetVec.z - transform.position.z);
		duration = dur;
		elapsedTime = 0.0f;
		rigState = RigState.Teleporting;
	}

	public void StartBump(Vector3 targVec, float dur){
		targetVec = targVec;
		startVec = transform.position;
		distanceVec = new Vector3 (targetVec.x - transform.position.x, transform.position.y, targetVec.z - transform.position.z);
		duration = dur;
		elapsedTime = 0.0f;
		rigState = RigState.Bump;
	}

	void Update () {

		//store the current position of the rig for changes
		Vector3 curPos = transform.position;

		switch (rigState) {

		case RigState.Climbing:
			//if climbing and still below threshold from target high
			if (curPos.y < targetY - threshold) {
				//keep climbing
				curY = climbing (start, distance, elapsedTime, duration);
				elapsedTime += Time.deltaTime;
				curPos.y = curY;
			} else {
				//stop climbing
				rigState = RigState.Floating;
				curY = targetY;
				curPos.y = curY;
			}
			break;

		case RigState.Falling:
			//if falling and still above threshold distance from ground
			if (curPos.y > targetY + threshold) {
				//keep falling
				curY = falling (start, distance, elapsedTime, duration);
				elapsedTime += Time.deltaTime;
				curPos.y = curY;
			} else {
				//stop fall
				rigState = RigState.Grounded;
				curY = targetY;
				curPos.y = curY;
			}
			break;
		
		//used for moving the user over time to a teleport location
		case RigState.Teleporting:
			float dist = Vector3.Distance (transform.position, targetVec);
			//if not cloe enough to teleport spot
			if (dist > threshold) {
				// keep moving
				curX = teleporting (startVec.x, distanceVec.x, elapsedTime, duration);
				curY = teleporting (startVec.y, distanceVec.y, elapsedTime, duration);
				curZ = teleporting (startVec.z, distanceVec.z, elapsedTime, duration);
				elapsedTime += Time.deltaTime;
				curPos.x = curX;
				curPos.y = curY;
				curPos.z = curZ;
			} else {
				//stop teleporting
				rigState = RigState.Grounded;
				curX = targetVec.x;
				curY = targetVec.y;
				curZ = targetVec.z;
				curPos.x = curX;
				curPos.y = curY;
				curPos.z = curZ;
			}
			break;
		}	

		if (transform.parent == null) {
			//if the position of the rig has changed
			if (transform.position != curPos) {
				transform.position = curPos;
			}
		}

		if (fadingIn) {
			fadingOut = false;
			curAlpha = fade(startAlpha, distAlpha, elapsedTimeAlpha, duration);
			elapsedTimeAlpha += Time.deltaTime;
			Color newColor = new Color (Panel_Fade.GetComponent<Image> ().color.r, Panel_Fade.GetComponent<Image> ().color.g, Panel_Fade.GetComponent<Image> ().color.b, curAlpha);
			if (newColor.a < 0.1f) {
				newColor.a = 0.0f;
				fadingIn = false;
			}
			Panel_Fade.GetComponent<Image> ().color = newColor;
		}

		if (fadingOut) {			
			fadingIn = false;
			curAlpha = fade(startAlpha, distAlpha, elapsedTimeAlpha, duration);
			elapsedTimeAlpha += Time.deltaTime;
			Color newColor = new Color (Panel_Fade.GetComponent<Image> ().color.r, Panel_Fade.GetComponent<Image> ().color.g, Panel_Fade.GetComponent<Image> ().color.b, curAlpha);
			if (newColor.a > 0.9f) {
				newColor.a = 1.0f;
				fadingOut = false;
			}
			Panel_Fade.GetComponent<Image> ().color = newColor;
		}
	}

	public void SetToOut(){
		fadingIn = false;
		fadingOut = false;
		Color newColor = new Color (Panel_Fade.GetComponent<Image> ().color.r, Panel_Fade.GetComponent<Image> ().color.g, Panel_Fade.GetComponent<Image> ().color.b, 1.0f);
		Panel_Fade.GetComponent<Image> ().color = newColor;
	}

	public void SetToIn(){
		fadingIn = false;
		fadingOut = false;
		Color newColor = new Color (Panel_Fade.GetComponent<Image> ().color.r, Panel_Fade.GetComponent<Image> ().color.g, Panel_Fade.GetComponent<Image> ().color.b, 0.0f);
		Panel_Fade.GetComponent<Image> ().color = newColor;
	}

	public void BeginFadeOut(){
		Startfade (1.0f, 1.0f);
		fadingOut = true;
		fadingIn = false;

	}

	public void BeginFadeIn(){
		Startfade (0.0f, 1.0f);
		fadingIn = true;
		fadingOut = false;
	}

	public void Startfade(float tAlpha, float dur){		
		startAlpha = Panel_Fade.GetComponent<Image> ().color.a;
		targAlpha = tAlpha;
		distAlpha = targAlpha - startAlpha;
		duration = dur;
		elapsedTimeAlpha = 0.0f;
	}
}
