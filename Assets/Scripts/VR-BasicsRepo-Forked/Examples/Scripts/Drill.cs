using UnityEngine;
using System.Collections;

public class Drill : VRBasics_Grabbable {

	public Animator animator;
	public GameObject spindal;
	private bool spin;
	private float speed = 0.0f;

	// Update is called once per frame
	void Update () {

		//if the drill is being grabbed
		//and there is a controller grabbing it
		if (GetIsGrabbed () && GetControllerGrab ()) {

			//if the trigger of the controller that is grabbing the drill is pressed
			if (GetControllerGrab ().GetisTrigger ()) {

				//animate the trigger to the down state
				animator.SetBool ("trigger", true);

				spin = true;

				speed += 0.1f;
				speed = Mathf.Clamp (speed, 0.0f, 20.0f);

			} else {

				//animate the trigger to the up state
				animator.SetBool ("trigger", false);

				spin = false;
				speed -= 0.1f;
				speed = Mathf.Clamp (speed, 0.0f, 20.0f);
			}
		
		//if the drill is not being grabbed
		} else {

			//keep the trigger in the up state
			animator.SetBool ("trigger", false);
		
			spin = false;
			speed -= 0.5f;
			speed = Mathf.Clamp (speed, 0.0f, 20.0f);
		}
			
		Spin ();
	}


	void Spin(){

		spindal.transform.Rotate (new Vector3 (speed, 0, 0), Space.Self);
	}
}
