
//=================== VRBasics_TouchAndPushManager ==========================
//
// gets attached to the Toucher prefab object which should be a child object of a Controller
// handles the effects of the trigger collider for the Toucher
//
//=========================== by Zac Zidik ====================================

using UnityEngine;
using System.Collections;

public class VRBasics_TouchAndPushManager : MonoBehaviour {

	//an object being touched
	public GameObject touchedObject;

	public virtual void OnTriggerEnter(Collider other){
		if (other.gameObject.GetComponent<VRBasics_Touchable> ()) {
			
			//tell the touched object to change to the touch material
			other.gameObject.GetComponent<VRBasics_Touchable> ().DisplayTouchedColor ();
			//indicate the object is being touched 
			other.gameObject.GetComponent<VRBasics_Touchable> ().SetIsTouched(true);
			//pass the controller doing the touching to the object
			VRBasics_Controller controller = this.gameObject.transform.parent.gameObject.GetComponent<VRBasics_Controller> ();
			other.gameObject.GetComponent<VRBasics_Touchable> ().SetControllerTouch (controller);

			//is there a sound to play when the object is touched
			//only play sound on initial touch (not repeatedly)
			if (other.gameObject.GetComponent<VRBasics_Touchable> ().touchAudio && touchedObject != other.gameObject) {
				other.gameObject.GetComponent<VRBasics_Touchable> ().touchAudio.Play ();
			}

			//define the touchObject
			touchedObject = other.gameObject;

			//if the object being touched uses haptics
			if(other.gameObject.GetComponent<VRBasics_Touchable>().useHapticTouch){
				controller.HapticPulse (other.gameObject.GetComponent<VRBasics_Touchable>().hapticDuration);
			}
		}
	}

	public virtual void OnTriggerExit(Collider other){
		if (other.gameObject.GetComponent<VRBasics_Touchable> ()) {
			
			//tell the touched object to change to the touch material
			other.gameObject.GetComponent<VRBasics_Touchable> ().DisplayUntouchedColor();
			//indicate the object is being touched
			other.gameObject.GetComponent<VRBasics_Touchable> ().SetIsTouched(false);
			//tell the object no controller is touching it
			VRBasics_Controller controller = this.gameObject.transform.parent.gameObject.GetComponent<VRBasics_Controller>();
			other.gameObject.GetComponent<VRBasics_Touchable> ().SetControllerTouch (null);
			//define the touchObject
			touchedObject = null;

			//if the object is pushable by default
			if (other.gameObject.GetComponent<VRBasics_Touchable> ().isPushable) {

				//make sure collsions between the Pusher and this object are enabled
				//it may have been disable by the GrabManager if the object was grabbed
				if (other.gameObject.GetComponent<VRBasics_Grabbable> ()) {
					if (!other.gameObject.GetComponent<VRBasics_Grabbable> ().GetIsGrabbed()) {
						controller.CollideWithPushers (other.gameObject);
					}
				} else {
					controller.CollideWithPushers (other.gameObject);
				}
			}
		}
	}
}