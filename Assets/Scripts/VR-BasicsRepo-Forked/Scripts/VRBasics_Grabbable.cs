
//======================= VRBasics_Grabbable ==================================
//
// an extention of the VRBasics_Touchable class
// place on objects that can not only be touched but also grabbed
//
//=========================== by Zac Zidik ====================================
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using System.Collections;

public class VRBasics_Grabbable : VRBasics_Touchable {

	//options on what type of joint is created betwen the controller and the grabbed object
	public enum JointTypes{Fixed,Spring,Child};
	//what type of joint is created betwen the controller and the grabbed object
	public JointTypes jointType;
	//will this object become a child of the grabber when grabbed
	public bool reparent = false;
	//will the object be reparented to the world after it is dropped, otherwise it will retunr to it's orginal parent
	public bool reparentToWorldWhenDropped = false;
	//does the object's rigid body become kinematic when grabbed
	public bool kinematicWhenGrabbed = false;
	//which button on the controller is used to grab this object
	public GrabButtons grabButton;
	//state of grabbed object
	private bool isGrabbed = false;
	//moves the grabbed object a little closer to the grabber
	public bool isGrabnetized = false;
	//controls how much closer, between 0.0 and 1.0
	public float grabnetStrength = 1.0f;
	//amount of force to break the object from the grab
	public float grabBreakforce = Mathf.Infinity;
	//hides the rendered child objects of the controller (when grabbing an object) if true
	public bool grabHidesController = false;
	//determines if the object can be thrown
	public bool isThrowable = false;
	//a seperate attach point game object
	//used to orient a grabbable object into a specific position relative to the controller when grabbed
	public GameObject attach;
	//used to a reference to the controller that is grabbing the object
	private VRBasics_Controller controller_grab;
	//store the starting parent of the object, cause it might change when it is grabbed
	private GameObject originalParent;
	//store the properties of original rigid body
	//do this because if the object is used a child in a connectorChild scenerio
	//the rigidbody of the child is removed when reparented, and then put back when it is grabbed again
	private RigidBodyProperties rbProperties = new RigidBodyProperties();
	//an transform used to make a controllers hand model a child of
	public GameObject handModelParentRight;
	public GameObject handModelParentLeft;
	//a sound played when grab is made
	public AudioSource grabAudio;
	//a sound played when grab is released is made
	public AudioSource releasedAudio;
	//a sound played when an object is thrown
	public AudioSource throwAudio;

	new void Start(){
		base.Start ();

		SetOriginalParent ();

		if (transform.GetComponent<Rigidbody> ()) {
			rbProperties.mass = transform.GetComponent<Rigidbody> ().mass;
			rbProperties.useGravity = transform.GetComponent<Rigidbody> ().useGravity;
			rbProperties.isKinematic = transform.GetComponent<Rigidbody> ().isKinematic;
		}
	}

	public void SetOriginalParent(){
		//if the object's parent is the world
		if (transform.parent == null) {
			originalParent = null;
		} else {
			//store the parent object's parent
			originalParent = transform.parent.gameObject;
		}
	}

	public RigidBodyProperties GetRigidBodyProperties(){
		return rbProperties;
	}

	public GameObject GetOriginalParent(){
		return originalParent;
	}

	public void SetControllerGrab(VRBasics_Controller c){
		controller_grab = c;
	}

	public VRBasics_Controller GetControllerGrab(){
		return controller_grab;
	}

	public bool GetIsGrabbed(){
		return isGrabbed;
	}

	public void SetIsGrabbed(bool g){
		isGrabbed = g;
	}

	void FixedUpdate(){
		//if the object is not grabbed, or touched and is pushable by default but is not pushable
		if (!isGrabbed && !isTouched && isPushableDefault && !isPushable) {
			//make it pushable
			SetIsPushable (true);
		}

		//if the object is currently being grabbed
		if (isGrabbed) {
			if (kinematicWhenGrabbed) {
				if (transform.GetComponent<Rigidbody> () && transform.GetComponent<Rigidbody> ().isKinematic != true) {
					transform.GetComponent<Rigidbody> ().isKinematic = true;
				}
			}

		//if the object is currently not being grabbed
		} else {
			foreach(Transform child in transform){

				//does the object have a connector attached to it
				if(child.GetComponent<VRBasics_Connector>()){
					//if the connector is not currently connected
					if (!child.GetComponent<VRBasics_Connector> ().GetIsConnected ()) {
						if (transform.GetComponent<Rigidbody> () && transform.GetComponent<Rigidbody> ().isKinematic != false) {
							transform.GetComponent<Rigidbody> ().isKinematic = false;
						}
					}
				}else{
					if (transform.GetComponent<Rigidbody> () && transform.GetComponent<Rigidbody> ().isKinematic != false) {
						transform.GetComponent<Rigidbody> ().isKinematic = false;
					}
				}
			}
		}
	}
}

public class RigidBodyProperties{
	public float mass = 1.0f;
	public bool useGravity = true;
	public bool isKinematic = false;
}
