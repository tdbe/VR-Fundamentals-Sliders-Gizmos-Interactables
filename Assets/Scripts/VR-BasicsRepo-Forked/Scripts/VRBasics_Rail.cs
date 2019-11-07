
//========================== VRBasics_Rail ====================================
//
// draws a gizmo that indicates the position and length of a rail prefab
// rails provide a solid mount for the child slider to move back and forth along a single axis
//
//=========================== by Zac Zidik ====================================
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using System.Collections;

//typically a kinematic rigidbody
//serves as a stable object for a slider object to move across
[ExecuteInEditMode]
[RequireComponent (typeof(Rigidbody))]
public class VRBasics_Rail : MonoBehaviour {
	public Transform minLimitPos;
	public Transform maxLimitPos;
	Vector3 minLimitPosOld;
	Vector3 maxLimitPosOld;
	public Color gizmoRuntimeColor = Color.white;
	[Range(0.01f,10)]
	public float gizmoResolution_relative = 1f;
	[Range(0.01f,10)]
	public float gizmoSize_relative = 1f;


    //the gameobject the moves along the rail
    //public GameObject slider;
    public VRBasics_Slider _slider;
	public VRBasics_Slider slider {get{if(_slider == null) _slider = transform.GetChild(0).GetComponent<VRBasics_Slider>(); return _slider;}}
	//the length of the rail
	public float length = 1.0f;
	float lengthOld;
	//the location of the anchor along the rail between 0.0 and 1.0
	public float anchor = 0.5f;
	float anchorOld;
	//the amount the anchor has moved from the orginal position
	public float anchorMove;

	private void OnEnable() {
		
	}

	private void Update() {
		if(length != lengthOld || anchor != anchorOld )//|| minLimitPos.position != minLimitPosOld || maxLimitPos.position != maxLimitPosOld ){
		{
			UpdateChildren();
			slider.UpdateChildren();
		}
		lengthOld = length;
		anchorOld = anchor;
		minLimitPosOld = minLimitPos.position;
		maxLimitPosOld = maxLimitPos.position;
	}
	
	public void SetSliderPosition(float newVal){
		slider.SetPosition(newVal); 
	}

	public void UpdateChildren(){

		//an empty game object used to aid in positioning
		//GameObject dummyTrans = GetDummy();
		Vector3 dummyPos = Vector3.zero;

		//reference to the configurable joint the gizmo displays
		ConfigurableJoint configJoint = slider.TargetConfigurableJoint;//.gameObject.GetComponent<ConfigurableJoint> ();

		dummyPos = transform.position;
		dummyPos += transform.up * anchorMove;

		minLimitPos.position = dummyPos + (transform.up.normalized* configJoint.linearLimit.limit);
		maxLimitPos.position = dummyPos - (transform.up.normalized * configJoint.linearLimit.limit);
	}

	/*
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
	}*/

	public void SetAnchorMove(float m){
		//store the value of the distance the achor has moved
		anchorMove = m;
		//reposition the connected anchor on the slider joint
		slider.SetConnectedAnchorPos ();
	}
	
}


