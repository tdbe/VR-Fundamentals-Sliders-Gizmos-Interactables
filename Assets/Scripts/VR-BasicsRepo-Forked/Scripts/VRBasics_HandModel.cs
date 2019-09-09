using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRBasics_HandModel : MonoBehaviour {

	public VRBasics_Controller controller;
	public Animator animator;
	public VRBasics_GrabManager grabManager;
	private Vector3 origlLocalPosition;
	private Vector3 origLocalRotation;

	void Start () {

		origlLocalPosition = transform.localPosition;
		origLocalRotation = transform.localEulerAngles;
	}

	public void Reparent(GameObject newParent){
		if (transform.parent != newParent.transform) {
			transform.parent = newParent.transform;
			transform.localPosition = Vector3.zero;
			transform.localEulerAngles = Vector3.zero;
		}
	}

	public void ReparentToController(){
		if (transform.parent != controller.transform) {
			transform.parent = controller.transform;
			transform.localPosition = origlLocalPosition;
			transform.localEulerAngles = origLocalRotation;
		}
	}

	void Update () {

		//HOW THE BUTTONS ON THE CONTROLLER EFFECT THE HAND MODEL
		//when the trigger button is being held
		//usually effects pointer finger animations
		if (controller.GetisTrigger ()) {		

			animator.SetBool ("trigger", true);

		} else if (!controller.GetisTrigger ()){

			animator.SetBool ("trigger", false);
		}		


		//when the grip button is being held
		//usually effect middle, ring and pinky finger animations
		if (controller.GetisGrip ()) {

			animator.SetBool ("grip", true);

		} else {

			animator.SetBool ("grip", false);
		}


		//when the touchpad is being pressed
		//usually effects thumb animations
		if (controller.GetisTouchPad ()) {

			animator.SetBool ("touchPad", true);

		} else {

			animator.SetBool ("touchPad", false);
		}	
	

		//HOLDING OBJECTS
		//example of specific objects that set custom animation states
		if (grabManager.grabbedObject && grabManager.grabbedObject.name == "drill") {

			animator.SetBool ("holdDrill", true);
		} else {
			
			animator.SetBool ("holdDrill", false);
		}
	}
}
