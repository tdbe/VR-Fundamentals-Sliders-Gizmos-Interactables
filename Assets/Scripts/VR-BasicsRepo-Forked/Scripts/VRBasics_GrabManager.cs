﻿
//====================== VRBasics_GrabManager ===============================
//
// handles all grabbing and ungrabbing of Grabbable objects by the Controller
// is attched to the Toucher prefab object which should be a child object of the Controller
// also handles Throwing of Grabbable objects
//
//=========================== by Zac Zidik ====================================

using UnityEngine;
using System.Collections;

public class VRBasics_GrabManager : MonoBehaviour {
	//an object being grabbed
	public GameObject grabbedObject;
	//a fixed joint to the grabbed object
	private FixedJoint joint_f;
	//a spring joint to the grabbed object
	private SpringJoint joint_s;
	//reference to the parent controller
	private GameObject Controller;

	//private GameObject grabbedObject_originalParent;

	void Awake(){
		//keep a reference to the parent controller
		Controller = this.gameObject.transform.parent.gameObject;
	}

	void Update(){

		//GRAB
		//if the Toucher is touching an a Touchable object and the object being touched is grabbale and not currently grabbed
		if (GetComponent<VRBasics_TouchAndPushManager>().touchedObject != null && GetComponent<VRBasics_TouchAndPushManager>().touchedObject.GetComponent<VRBasics_Grabbable>() && !GetComponent<VRBasics_TouchAndPushManager>().touchedObject.GetComponent<VRBasics_Grabbable>().GetIsGrabbed()) {
			//the grip of the controller is down and the object can be grabbed with the grip button
			if (Controller.GetComponent<VRBasics_Controller>().GetisGrip() && GetComponent<VRBasics_TouchAndPushManager>().touchedObject.GetComponent<VRBasics_Grabbable>().grabButton == GrabButtons.grip) {
				//if the controller is available to grab using this button
				if (!Controller.GetComponent<VRBasics_Controller> ().GetisGripGrab()) {
					//grab the object
					GrabObject (GrabButtons.grip);
				}
			
			//the touchpad of the controller is down and the object can be grabbed with the touchpad button
			}else if (Controller.GetComponent<VRBasics_Controller>().GetisTouchPad() && GetComponent<VRBasics_TouchAndPushManager>().touchedObject.GetComponent<VRBasics_Grabbable>().grabButton == GrabButtons.touchpad) {
				//if the controller is available to grab using this button
				if (!Controller.GetComponent<VRBasics_Controller> ().GetisTouchPadGrab()) {
					//grab the object
					GrabObject (GrabButtons.touchpad);
				}

			//the trigger of the controller is down and the object can be grabbed with the trigger button
			}else if (Controller.GetComponent<VRBasics_Controller>().GetisTrigger() && GetComponent<VRBasics_TouchAndPushManager>().touchedObject.GetComponent<VRBasics_Grabbable>().grabButton == GrabButtons.trigger) {
				//if the controller is available to grab using this button
				if (!Controller.GetComponent<VRBasics_Controller> ().GetisTriggerGrab()) {
					//grab the object
					GrabObject (GrabButtons.trigger);
				}
			}
		}

		//RELEASE
		//if the controller is cutrrently grabbing an object
		if (grabbedObject != null) {

			//JOINT BROKE
			//if the joint is gone (possibly because of breaking)
			//but the controller still thinks it is grabbing an object
			//for objects grabbed by a fixed joint
			if (grabbedObject.GetComponent<VRBasics_Grabbable> ().jointType == VRBasics_Grabbable.JointTypes.Fixed && joint_f == null) {

				UnGrabObject ();
			
			//for objects grabbed by a spring joint
			}else if (grabbedObject.GetComponent<VRBasics_Grabbable> ().jointType == VRBasics_Grabbable.JointTypes.Spring && joint_s == null) {

				UnGrabObject ();

			//the grip is up and the object can be grabbed with the grip button
			} else if (!Controller.GetComponent<VRBasics_Controller> ().GetisGrip() && grabbedObject.GetComponent<VRBasics_Grabbable> ().grabButton == GrabButtons.grip) {
				//let go of the object
				ReleaseObject ();

			//the touchpad is up and the object can be grabbed with the touchpad button
			} else if (!Controller.GetComponent<VRBasics_Controller> ().GetisTouchPad() && grabbedObject.GetComponent<VRBasics_Grabbable> ().grabButton == GrabButtons.touchpad) {
				//let go of the object
				ReleaseObject ();

			//the trigger is up and the object can be grabbed with the trigger button
			} else if (!Controller.GetComponent<VRBasics_Controller> ().GetisTrigger() && grabbedObject.GetComponent<VRBasics_Grabbable> ().grabButton == GrabButtons.trigger) {
				//let go of the object
				ReleaseObject ();
			}
		}
	}

	public GameObject GetDummy(){
		GameObject dummyTrans;
		//if one exist
		if (GameObject.Find ("Dummy")) {
			//get the Dummy transform object
			dummyTrans = GameObject.Find ("Dummy");
			//if one doesnt exist
		} else {
			//create one
			dummyTrans = new GameObject ();
			dummyTrans.name = "Dummy";
		}
		return dummyTrans;
	}

	void GrabObject(GrabButtons gb){

		//if not already grabbing and object
		if (joint_f == null && joint_s == null) {

			grabbedObject = GetComponent<VRBasics_TouchAndPushManager>().touchedObject;

			//add a rigidbody if one does not exist
			if (!grabbedObject.GetComponent<Rigidbody> ()) {

				grabbedObject.AddComponent<Rigidbody> ();
				grabbedObject.GetComponent<Rigidbody> ().mass = grabbedObject.GetComponent<VRBasics_Grabbable> ().GetRigidBodyProperties ().mass;
				grabbedObject.GetComponent<Rigidbody> ().useGravity = grabbedObject.GetComponent<VRBasics_Grabbable> ().GetRigidBodyProperties ().useGravity;
				grabbedObject.GetComponent<Rigidbody> ().isKinematic = grabbedObject.GetComponent<VRBasics_Grabbable> ().GetRigidBodyProperties ().isKinematic;
			}

			//set the velocity to zero to stop all movement of the object when grabbed
			grabbedObject.GetComponent<Rigidbody> ().velocity = Vector3.zero;

			//if this object has a seperate attach point object
			if (grabbedObject.GetComponent<VRBasics_Grabbable> ().attach != null) {

				//an empty game object used to aid in positioning
				GameObject dummy = GetDummy ();
				dummy.transform.position = grabbedObject.GetComponent<VRBasics_Grabbable> ().attach.transform.position;
				dummy.transform.rotation = grabbedObject.GetComponent<VRBasics_Grabbable> ().attach.transform.rotation;

				//reparent object to dummy
				grabbedObject.transform.parent = dummy.transform;

				//move the dummy to the position of the controller
				dummy.transform.position = transform.position;
				dummy.transform.rotation = transform.rotation;

				//reparent the object to it's original parent
				if (grabbedObject.GetComponent<VRBasics_Grabbable> ().GetOriginalParent() == null) {
					grabbedObject.transform.parent = null;
				} else {
					grabbedObject.transform.parent = grabbedObject.GetComponent<VRBasics_Grabbable> ().GetOriginalParent().transform;
				}

				//remove the dummy
				DestroyImmediate (dummy);
			
			//if not using an attach point object
			//check if this object is grabnetized
			}else if (grabbedObject.GetComponent<VRBasics_Grabbable> ().isGrabnetized) {
				
				//moves the grabbed object a little closer to the grabber before creating the joint between the two
				//this will prevent the joint from immediately breaking because of downward force put on the object by the grabber when sitting on a flat surface
				grabbedObject.transform.position = Vector3.Lerp (grabbedObject.transform.position, transform.position, grabbedObject.GetComponent<VRBasics_Grabbable> ().grabnetStrength);
			}

			//add a joint to the object
			if (grabbedObject.GetComponent<VRBasics_Grabbable> ().jointType == VRBasics_Grabbable.JointTypes.Fixed) {

				joint_f = grabbedObject.AddComponent<FixedJoint> ();

				//connect the joint to the grabber
				//this connection will disable collisions between the Toucher and the grabbed object and must be re-enabled later
				joint_f.connectedBody = GetComponent<Rigidbody> ();
				//the amount of force it will take to break this joint
				//for example when the object comes into contact with another object, like a table
				joint_f.breakForce = grabbedObject.GetComponent<VRBasics_Grabbable> ().grabBreakforce;


			} else if (grabbedObject.GetComponent<VRBasics_Grabbable> ().jointType == VRBasics_Grabbable.JointTypes.Spring) {

				joint_s = grabbedObject.AddComponent<SpringJoint> ();
				joint_s.spring = 1000.0f;
				joint_s.damper = 0.0f;
				joint_s.minDistance = 0.0f;
				joint_s.maxDistance = 0.0f;

				//connect the joint to the grabber
				//this connection will disable collisions between the Toucher and the grabbed object and must be re-enabled later
				joint_s.connectedBody = GetComponent<Rigidbody> ();
				//the amount of force it will take to break this joint
				//for example when the object comes into contact with another object, like a table
				joint_s.breakForce = grabbedObject.GetComponent<VRBasics_Grabbable> ().grabBreakforce;
			}

			//set the object state to grabbed
			grabbedObject.GetComponent<VRBasics_Grabbable> ().SetIsGrabbed(true);

			//when the controller is grabbing an object, it is no longer allowed to touch or push any other objects
			Controller.GetComponent<VRBasics_Controller> ().DisableTouchersAndPushers (true);

			//cancel collsions between the Pushers of the controller and the grabbed object
			//this will be re-enabled on trigger exit of the Toucher
			Controller.GetComponent<VRBasics_Controller> ().IgnorePushers(grabbedObject);

			//set the state of the button unable to grab again unitl it is released
			switch (gb) {
			case GrabButtons.grip:
				Controller.GetComponent<VRBasics_Controller> ().SetisGripGrab(true);
				break;
			case GrabButtons.touchpad:
				Controller.GetComponent<VRBasics_Controller> ().SetisTouchPadGrab(true);
				break;
			case GrabButtons.trigger:
				Controller.GetComponent<VRBasics_Controller> ().SetisTriggerGrab(true);
				break;
			}

			//does grabbing this object hide the controller
			if (grabbedObject.GetComponent<VRBasics_Grabbable> ().grabHidesController) {
				Controller.GetComponent<VRBasics_Controller>().HideController ();
			}

			//tell the grabbed object which controller is grabbing it
			grabbedObject.GetComponent<VRBasics_Grabbable> ().SetControllerGrab (Controller.GetComponent<VRBasics_Controller> ());

			//when using reparenting the grabbed object to the grabber
			if (grabbedObject.GetComponent<VRBasics_Grabbable> ().reparent) {
				//make the object's parent the Controller while it is being grabbed
				//this will eliminate the wobble in the fixed joint
				grabbedObject.transform.parent = Controller.transform;

				//if the grabbed obejct was part of connector child
				//disable the connection
				foreach (Transform child in grabbedObject.transform) {
					//does the object have a connector child attached to it
					if (child.GetComponent<VRBasics_ConnectorChild> ()) {
						//if the connector is currently connected
						if (child.GetComponent<VRBasics_ConnectorChild> ().GetIsConnected ()) {
							child.GetComponent<VRBasics_ConnectorChild> ().Detach ();
						}
					}
				}
			
			//if not using reparenting, the grabbed object should always return to its orginal parent when grabbed
			//this will cancel any new parent created by a possible grab of a VRBasics_ConnectorChild
			} else {
				
				//if the original parent object is the world
				if (grabbedObject.GetComponent<VRBasics_Grabbable> ().GetOriginalParent () == null) {
					if(grabbedObject.transform.parent != null){
						grabbedObject.transform.parent = null;
					}
				} else {
					if (grabbedObject.transform.parent != grabbedObject.GetComponent<VRBasics_Grabbable> ().GetOriginalParent ().transform) {
						grabbedObject.transform.parent = grabbedObject.GetComponent<VRBasics_Grabbable> ().GetOriginalParent ().transform;
					}
				}

				//if the grabbed obejct was part of connector child
				//disable the connection
				foreach (Transform child in grabbedObject.transform) {
					//does the object have a connector child attached to it
					if (child.GetComponent<VRBasics_ConnectorChild> ()) {
						//if the connector is currently connected
						if (child.GetComponent<VRBasics_ConnectorChild> ().GetIsConnected ()) {
							child.GetComponent<VRBasics_ConnectorChild> ().Detach ();
						}
					}
				}
			}

			//does controller grabbing the object have a hand model
			if (Controller.GetComponent<VRBasics_Controller> ().handModel != null) {

				//RIGHT HAND CONTROLLER
				//Controller (right) is for SteamVR and RightHandAnchor is for Oculus
				if (Controller.name == "Controller (right)" || Controller.name == "RightHandAnchor") {
					//does the object want to reparent the hand model
					if (grabbedObject.GetComponent<VRBasics_Grabbable> ().handModelParentRight != null) {

						//reparent the hand model to the grabbed object's hand model parent
						Controller.GetComponent<VRBasics_Controller> ().handModel.Reparent (grabbedObject.GetComponent<VRBasics_Grabbable> ().handModelParentRight);
					}

				//LEFT HAND CONTROLLER
				//Controller (left) is for SteamVR and LeftHandAnchor is for Oculus
				} else if (Controller.name == "Controller (left)" || Controller.name == "LeftHandAnchor") {
					//does the object want to reparent the hand model
					if (grabbedObject.GetComponent<VRBasics_Grabbable> ().handModelParentLeft != null) {

						//reparent the hand model to the grabbed object's hand model parent
						Controller.GetComponent<VRBasics_Controller> ().handModel.Reparent (grabbedObject.GetComponent<VRBasics_Grabbable> ().handModelParentLeft);
					}
				}
			}

			//play the grab sound
			if (grabbedObject.GetComponent<VRBasics_Grabbable> ().grabAudio) {
				grabbedObject.GetComponent<VRBasics_Grabbable> ().grabAudio.Play ();
			}
		}
	}

	void ReleaseObject(){

		if (joint_f != null) {
			//destroy the joint between the Controller and the grabbedObject
			Object.DestroyImmediate (joint_f);
			//clear the joint
			joint_f = null;

			//if the parent object's parent is the world
			if (grabbedObject.GetComponent<VRBasics_Grabbable> ().GetOriginalParent () == null) {
				if(grabbedObject.transform.parent != null){
					grabbedObject.transform.parent = null;
				}
			} else {
				if (grabbedObject.transform.parent != grabbedObject.GetComponent<VRBasics_Grabbable> ().GetOriginalParent ().transform) {
					grabbedObject.transform.parent = grabbedObject.GetComponent<VRBasics_Grabbable> ().GetOriginalParent ().transform;
				}
			}
		}

		if (joint_s != null) {
			//destroy the joint between the Controller and the grabbedObject
			Object.DestroyImmediate (joint_s);
			//clear the joint
			joint_s = null;
		}

		//play the release sound
		if (grabbedObject.GetComponent<VRBasics_Grabbable> ().releasedAudio) {
			grabbedObject.GetComponent<VRBasics_Grabbable> ().releasedAudio.Play ();
		}

		//THROW
		//check if object is throwable
		if (grabbedObject.GetComponent<VRBasics_Grabbable> ().isThrowable) {

			ThrowObject ();
		}
			
		UnGrabObject ();
	}

	void ThrowObject(){
		
		//get the rigid body of the currently grabbed object
		Rigidbody rigidbody = grabbedObject.GetComponent<Rigidbody> ();

		//look for possible orign of offset between the parent of the Controller for a more accurate physics sim
		Transform origin = Controller.GetComponent<VRBasics_Controller>().GetOrigin();

		//if some origin of offset can be determined
		if (origin != null) {
			//give some speed and rotation to the object
			rigidbody.velocity = origin.TransformVector (Controller.GetComponent<VRBasics_Controller>().GetVelocity());
			rigidbody.angularVelocity = origin.TransformVector (Controller.GetComponent<VRBasics_Controller>().GetAngularVelocity());
		} else {		
			//give some speed and rotation to the object
			rigidbody.velocity = Controller.GetComponent<VRBasics_Controller>().GetVelocity();
			rigidbody.angularVelocity = Controller.GetComponent<VRBasics_Controller>().GetAngularVelocity();
			Debug.Log ("V = " + rigidbody.velocity + "," + " AV = " + rigidbody.angularVelocity);
		}

		//cap the angular velocity
		rigidbody.maxAngularVelocity = rigidbody.angularVelocity.magnitude;

		//play the throw sound
		if (grabbedObject.GetComponent<VRBasics_Grabbable> ().throwAudio) {
			grabbedObject.GetComponent<VRBasics_Grabbable> ().throwAudio.Play ();
		}
	}

	void UnGrabObject(){
		
		//set the object state to not grabbed
		grabbedObject.GetComponent<VRBasics_Grabbable> ().SetIsGrabbed(false);

		//since the joint between the Toucher and the object canceled the collisions, we must re-enable them
		Physics.IgnoreCollision (GetComponent<Collider> (), grabbedObject.GetComponent<Collider> (), false);

		//if grabbing this object hid the controller
		if (grabbedObject.GetComponent<VRBasics_Grabbable> ().grabHidesController) {
			Controller.GetComponent<VRBasics_Controller> ().ShowController ();
		}

		//tell the grabbed object no controller is grabbing it
		grabbedObject.GetComponent<VRBasics_Grabbable> ().SetControllerGrab(null);

		if (grabbedObject.GetComponent<VRBasics_Grabbable> ().reparentToWorldWhenDropped) {

			grabbedObject.transform.parent = null;

		} else if (grabbedObject.GetComponent<VRBasics_Grabbable> ().reparent) {
			
			//if the parent object's parent is the world
			if (grabbedObject.GetComponent<VRBasics_Grabbable> ().GetOriginalParent () == null) {
				if(grabbedObject.transform.parent != null){
					grabbedObject.transform.parent = null;
				}
			} else {
				if (grabbedObject.transform.parent != grabbedObject.GetComponent<VRBasics_Grabbable> ().GetOriginalParent ().transform) {
					grabbedObject.transform.parent = grabbedObject.GetComponent<VRBasics_Grabbable> ().GetOriginalParent ().transform;
				}
			}
		}
			
		//does controller grabbing the object have a hand model
		if (Controller.GetComponent<VRBasics_Controller>().handModel != null) {

			//is the hand model's parent not the controller
			if (Controller.GetComponent<VRBasics_Controller>().handModel.transform.parent != Controller.GetComponent<VRBasics_Controller>().transform) {
				//return the hand to it's parent controller
				Controller.GetComponent<VRBasics_Controller>().handModel.ReparentToController ();
			}
		}

		//empty the grabbed object
		grabbedObject = null;

		//reactivate touch an push on the controller
		Controller.GetComponent<VRBasics_Controller> ().DisableTouchersAndPushers (false);
	}
}
