
//======================== VRBasics_RailEditor ================================
//
// custom editor for VRBasics_Rail class
// automatic handling of multi-object handling, undo and prefab overrides
//
//=========================== by Zac Zidik ====================================

using UnityEditor;
using UnityEngine;
using System.Collections;

[CustomEditor(typeof(VRBasics_Rail))]
[CanEditMultipleObjects]
public class VRBasics_RailEditor : Editor {

    SerializedProperty drawCircleGizmosProp;
    SerializedProperty drawCircleGizmos2Prop;
    SerializedProperty minLimitPosProp;
	SerializedProperty maxLimitPosProp;
	SerializedProperty sliderProp;
	SerializedProperty lengthProp;
	SerializedProperty anchorProp;

	void OnEnable(){
        //set up serialized properties

        drawCircleGizmosProp = serializedObject.FindProperty ("drawCircleGizmos"); 
        drawCircleGizmos2Prop = serializedObject.FindProperty ("drawCircleGizmos2"); 
        minLimitPosProp = serializedObject.FindProperty ("minLimitPos");
        maxLimitPosProp = serializedObject.FindProperty ("maxLimitPos"); 
		sliderProp = serializedObject.FindProperty ("_slider"); 
		lengthProp = serializedObject.FindProperty ("length");
		anchorProp = serializedObject.FindProperty ("anchor");

		VRBasics_Rail rail = (VRBasics_Rail) target;
		//move anchor if not in correct place
		float move = (rail.anchor * rail.length) - (rail.length * 0.5f);
		if (rail.anchorMove != move) {
			rail.SetAnchorMove (move);
		}
	}

	public override void OnInspectorGUI ()
	{
		//always update serialized properties at start of OnInspectorGUI
		serializedObject.Update ();

		//start listening for changes in inspector values
		EditorGUI.BeginChangeCheck ();

        //display serialized properties in inspector
        

        //EditorGUILayout.PropertyField (drawCircleGizmosProp, new GUIContent ("Slider drawCircleGizmos"));
        //EditorGUILayout.PropertyField (drawCircleGizmos2Prop, new GUIContent ("Slider drawCircleGizmos2"));
        EditorGUILayout.PropertyField (minLimitPosProp, new GUIContent ("Slider minLimitPos"));
        EditorGUILayout.PropertyField (maxLimitPosProp, new GUIContent ("Slider maxLimitPos"));
		EditorGUILayout.PropertyField (sliderProp, new GUIContent ("Slider Object"));
		EditorGUILayout.PropertyField (lengthProp, new GUIContent ("Length"));
		EditorGUILayout.Slider (anchorProp, 0.0f, 1.0f, new GUIContent ("Anchor"));

		//if there were any changes in inspector values
		if (EditorGUI.EndChangeCheck ()) {

			//apply changes to serialized properties
			serializedObject.ApplyModifiedProperties();

			VRBasics_Rail rail = (VRBasics_Rail) target;
			//move anchor if not in correct place
			float move = (rail.anchor * rail.length) - (rail.length * 0.5f);
			if (rail.anchorMove != move) {
				rail.SetAnchorMove (move);
			}

			//reference to the slider
			VRBasics_Slider _slider = rail._slider.GetComponent<VRBasics_Slider> ();
			//adjust linear limit to match the length of the rail
			_slider.SetLinearLimit ();
		}
	}

	void OnSceneGUI() {
		/*
		//reference to the class of object used to display gizmo
		VRBasics_Rail rail = (VRBasics_Rail) target;

		//reference to the slider
		VRBasics_Slider _slider = rail._slider.GetComponent<VRBasics_Slider> ();

		//use the inspector to postion the slider along the rail
		_slider.EditorSetPosition ();

		//calculate the percentage of the slider along the rail
		_slider.CalcPercentage ();

		//DRAW RAIl
		rail.DrawGizmo(null);

		//DRAW SLIDER
		_slider.DrawGizmo(null);
		*/
	}
}
