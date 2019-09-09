
//===================== VRBasics_ConnectorChild ===============================
//
// This is similar to VRBasics_Connector class, however when a connection is made here
// the male object becomes a child of the female object.
//
// ConnectorChild objects are used to attach a male object as a child to a female object.
// For a connection to be made, the connectors must be of opposite type (male / female).
// They must also share a coupleID.
//
// Once a connection if made, the male's rigid body is removed so it is no longer a factor in physics.
//
// The connection can be broken by grabbing the male child object. It will then return to it's orginal parent 
// and it's original rigid body will be restored.
//
//=========================== by Zac Zidik ====================================

#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using System.Collections;

public class VRBasics_ConnectorChild : MonoBehaviour {

	public enum connectorType{male,female}

	//male or female
	public connectorType type;
	//a compatible connector currently attached to (male and female)
	private GameObject attachedTo;
	//indication that the connector is connected to a compatible connector (male and female)
	public bool isConnected = false;
	//a way to identify compatible connectors (male and female)
	public int coupleID = 0;
	//the original parent of the parent object of the connector (male and female)
	//this needed because when a connection is made the connector repositions it's parent using a dummy parent
	//after positioning, the connector's parent is returned to it's orginal parent
	private GameObject originalParent;
	//a compatible connector currently touching (male and female)
	private GameObject touching;
	//when connected, is the mass from the male's removed rigid body added to the parent female's rigid body
	public bool addMassToParent;
	//a sound played when connection is made
	public AudioSource attachAudio;
	//a sound played when a connection is broken
	public AudioSource detachAudio;

	void Start(){

		//if the parent object's parent is the world
		if (transform.parent.transform.parent == null) {
			originalParent = null;
		} else {
			//store the parent object's parent
			originalParent = transform.parent.transform.parent.transform.gameObject;
		}
	}

	public GameObject GetOriginalParent(){
		return originalParent;
	}

	void AttachMaleToFemale(){
		//define the attached connector
		//male
		attachedTo = touching;
		//female
		attachedTo.GetComponent<VRBasics_ConnectorChild> ().attachedTo = this.gameObject;

		//cancel any collision between the parents
		if (transform.parent.GetComponent<Collider> () && attachedTo.transform.parent.GetComponent<Collider> ()) {
			Physics.IgnoreCollision (transform.parent.GetComponent<Collider> (), attachedTo.transform.parent.GetComponent<Collider> ());
		}

		//set the male position relative to the connected female
		PositionMale ();

		//indicate the connectors are connected to a compatible connector
		//male
		SetIsConnected(true);
		//female
		attachedTo.GetComponent<VRBasics_ConnectorChild>().SetIsConnected(true);

		//kill all velocity as a result of connection
		//male
		if (transform.parent.GetComponent<Rigidbody> ()) {
			transform.parent.GetComponent<Rigidbody> ().velocity = Vector3.zero;
			transform.parent.GetComponent<Rigidbody> ().angularVelocity = Vector3.zero;
		}
		//female
		if (attachedTo.transform.parent.GetComponent<Rigidbody> ()) {
			attachedTo.transform.parent.GetComponent<Rigidbody> ().velocity = Vector3.zero;
			attachedTo.transform.parent.GetComponent<Rigidbody> ().angularVelocity = Vector3.zero;
		}

		if (transform.parent.GetComponent<VRBasics_Grabbable> ()) {
			transform.parent.GetComponent<VRBasics_Grabbable> ().SetIsGrabbed (false);
			transform.parent.GetComponent<VRBasics_Grabbable> ().DisplayUntouchedColor ();
		}

		if (attachedTo.transform.parent.GetComponent<VRBasics_Grabbable> ()) {
			attachedTo.transform.parent.GetComponent<VRBasics_Grabbable> ().DisplayUntouchedColor ();
		}

		//remove the rigidbody of the male
		if (transform.parent.GetComponent<Rigidbody> ()) {
			Destroy (transform.parent.GetComponent<Rigidbody> ());
		}

		//make the male object a child of the female object
		transform.parent.transform.parent = attachedTo.transform.parent.transform;

		//play the attach audio
		if (attachAudio) {
			attachAudio.Play ();
		}
	}

	public void ConnectTo() {

		//all connections are controlled through the male connector
		//if currently touching a compatible connector
		//and not in a connection time out
		if (type == connectorType.male && touching != null) {

			//is the male grabbale
			if (transform.parent.GetComponent<VRBasics_Grabbable> ()) {

				//is the male not currently being grabbed
				if (!transform.parent.GetComponent<VRBasics_Grabbable> ().GetIsGrabbed ()) {

					AttachMaleToFemale ();
				} 
			
			//if the male is not grabbable
			} else {
				AttachMaleToFemale ();
			}
		}
	}

	public void Detach(){
		if (attachedTo != null) {
			attachedTo.GetComponent<VRBasics_ConnectorChild> ().SetIsConnected (false);
			attachedTo.GetComponent<VRBasics_ConnectorChild> ().SetAttachedToNull ();

			attachedTo = null;
			isConnected = false;

			//play the detach audio
			if (detachAudio) {
				detachAudio.Play ();
			}
		}
	}

	#if UNITY_EDITOR
	//label with connector information in scene editor
	public void DrawGizmo(){

		GUIStyle style = new GUIStyle();
		style.fontSize = 12;
		style.alignment = TextAnchor.MiddleCenter;


		if (type == connectorType.male) {

			style.normal.textColor = Color.cyan;
			Handles.Label (transform.position, "Male Connector Child (" + coupleID.ToString () + ")", style);


		} else if (type == connectorType.female) {

			style.normal.textColor = Color.magenta;
			Handles.Label (transform.position, "Female Connector Child (" + coupleID.ToString () + ")", style);
		}
	}
	#endif

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

	void OnTriggerEnter(Collider other){

		//if both objects in this collision are connectors
		if (GetComponent<VRBasics_ConnectorChild> () && other.GetComponent<VRBasics_ConnectorChild> ()) {

			//if a male connector comes into contact with a female connector who share a coupleID
			//and niether are currently connected to another connector
			if (type == connectorType.male &&
				other.GetComponent<VRBasics_ConnectorChild> ().type == connectorType.female &&
				coupleID == other.GetComponent<VRBasics_ConnectorChild> ().coupleID &&
				!isConnected &&
				!other.GetComponent<VRBasics_ConnectorChild> ().isConnected) {

				//let both connectors know which game object they are touching
				//male
				SetTouching (other.gameObject);
				//female
				other.GetComponent<VRBasics_ConnectorChild> ().SetTouching (this.gameObject);

				//male
				if (transform.parent.gameObject.GetComponent<VRBasics_Touchable> ()) {
					transform.parent.gameObject.GetComponent<VRBasics_Touchable> ().DisplayTouchedColor ();
				}

				//female
				if (other.transform.parent.gameObject.GetComponent<VRBasics_Touchable> ()) {
					other.transform.parent.gameObject.GetComponent<VRBasics_Touchable> ().DisplayTouchedColor ();
				}
			}
		}
	}

	void OnTriggerExit(Collider other){

		//if both objects in this collision are connectors
		if (GetComponent<VRBasics_ConnectorChild> () && other.GetComponent<VRBasics_ConnectorChild> ()) {

			//if a male connector exits contact with a female connector who share a coupleID
			if (type == connectorType.male &&
				other.GetComponent<VRBasics_ConnectorChild> ().type == connectorType.female &&
				coupleID == other.GetComponent<VRBasics_ConnectorChild> ().coupleID) {

				//let both connectors know they are not touching any compatible connectors
				//male
				SetTouching (null);
				//female
				other.GetComponent<VRBasics_ConnectorChild> ().SetTouching(null);

				//male
				if (transform.parent.gameObject.GetComponent<VRBasics_Touchable> ()) {
					transform.parent.gameObject.GetComponent<VRBasics_Touchable> ().DisplayUntouchedColor ();
				}

				//female
				if (other.transform.parent.gameObject.GetComponent<VRBasics_Touchable> ()) {
					other.transform.parent.gameObject.GetComponent<VRBasics_Touchable> ().DisplayUntouchedColor ();
				}
			}
		}
	}

	void Update(){

		ConnectionCheck ();
	}

	//if the male's parent's rigid body is kinematic
	//then the female assumes the position of the male
	void PositionFemale(){

		//if the male's parent's rigid body is kinematic
		if (attachedTo.transform.parent.GetComponent<Rigidbody> ().isKinematic) {

			//an empty game object used to aid in positioning
			GameObject dummy = GetDummy ();
			dummy.transform.position = transform.position;
			dummy.transform.rotation = transform.rotation;

			//make the parent of the parent object for the male connector the dummy
			transform.parent.transform.parent = dummy.transform;

			//move the dummy to the position and rotation of the female connector
			dummy.transform.rotation = attachedTo.transform.rotation;
			dummy.transform.position = attachedTo.transform.position;

			//return the parent of the parent object of the male connector to the original parent
			if (originalParent == null) {
				transform.parent.transform.parent = null;
			} else {
				transform.parent.transform.parent = originalParent.transform;
			}

			//remove the dummy
			DestroyImmediate (dummy);
		}
	}

	//if the male's parent's rigid body is not kinematic
	//then the male assumes the position of the female
	void PositionMale(){

		//if the male's parent's rigid body is not kinematic
		if (transform.parent.GetComponent<Rigidbody> () && !transform.parent.GetComponent<Rigidbody> ().isKinematic) {

			//an empty game object used to aid in positioning
			GameObject dummy = GetDummy ();
			dummy.transform.position = transform.position;
			dummy.transform.rotation = transform.rotation;

			//make the parent of the parent object for the male connector the dummy
			transform.parent.transform.parent = dummy.transform;

			//move the dummy to the position and rotation of the female connector
			dummy.transform.rotation = attachedTo.transform.rotation;
			dummy.transform.position = attachedTo.transform.position;

			//return the parent of the parent object of the male connector to the original parent
			if (originalParent == null) {
				transform.parent.transform.parent = null;
			} else {
				transform.parent.transform.parent = originalParent.transform;
			}

			//remove the dummy
			DestroyImmediate (dummy);
		}
	}

	public GameObject GetAttachedTo(){
		return attachedTo;
	}

	public void SetAttachedToNull(){
		attachedTo = null;
	}

	public bool GetIsConnected(){
		return isConnected;
	}

	public void SetIsConnected(bool c) {
		isConnected = c;
	}

	public void SetTouching(GameObject t) {
		touching = t;
	}

	void ConnectionCheck(){
		//CHECK FOR CONNECTIONS
		//check if the parent object of the connector is grabbable
		if (transform.parent.GetComponent<VRBasics_Grabbable> ()) {

			//is not currently grabbed
			if (!transform.parent.GetComponent<VRBasics_Grabbable> ().GetIsGrabbed ()) {

				//is not currently attached to a compatible connector but is touching one
				if (attachedTo == null && touching != null) {

					//if the connector it is touching is also not attached
					if (touching.GetComponent<VRBasics_ConnectorChild> ().attachedTo == null) {

						//make a connection
						ConnectTo ();
					}
				}

			//is currently grabbed
			} else if (transform.parent.GetComponent<VRBasics_Grabbable> ().GetIsGrabbed ()) {

				//is not currently attached to a compatible connector but is touching one
				if (attachedTo == null && touching != null) {

					//check if the parent object of the touching connector is grabbable
					if (touching.transform.parent.GetComponent<VRBasics_Grabbable> ()) {

						//if the connector it is touching is not currently grabbed
						if (!touching.transform.parent.GetComponent<VRBasics_Grabbable> ().GetIsGrabbed ()) {

							//if the connector it is touching is also not attached
							if (touching.GetComponent<VRBasics_ConnectorChild> ().attachedTo == null) {

								//make a connection
								ConnectTo ();
							}
						}

					//if the parent object of the touching connector is not grabbable
					} else {

						//if the connector it is touching is also not attached
						if (touching.GetComponent<VRBasics_ConnectorChild> ().attachedTo == null) {

							//make a connection
							ConnectTo ();
						}
					}
				}
			}

		//if the parent object is not grabbable
		} else {

			//is not currently attached to a compatible connector but is touching one
			if (attachedTo == null && touching != null) {

				//if the connector it is touching is also not attached
				if (touching.GetComponent<VRBasics_ConnectorChild> ().attachedTo == null) {

					//make a connection
					ConnectTo ();
				}
			}
		}
	}
}
