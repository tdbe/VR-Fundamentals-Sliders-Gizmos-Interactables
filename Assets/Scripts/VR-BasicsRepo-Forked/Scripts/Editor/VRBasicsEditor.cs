
//========================== VRBasicsEditor ===================================
//
// custom editor for VRBasics class
// automatic handling of multi-object handling, undo and prefab overrides
//
//=========================== by Zac Zidik ====================================

using UnityEditor;
using UnityEngine;
using System.Collections;

[CustomEditor(typeof(VRBasics))]
[CanEditMultipleObjects]
public class VRBasicsEditor : Editor {

	SerializedProperty autoVRTypeProp;
	SerializedProperty vrTypeProp;
	SerializedProperty ignoreCollisionsLayerProp;
	SerializedProperty renderScaleProp;


	void OnEnable(){
		//set up serialized properties
		autoVRTypeProp = serializedObject.FindProperty ("autoVRType");
		vrTypeProp = serializedObject.FindProperty ("vrType");
		ignoreCollisionsLayerProp = serializedObject.FindProperty ("ignoreCollisionsLayer");
		renderScaleProp = serializedObject.FindProperty ("renderScale");
	}

	public override void OnInspectorGUI ()
	{
		//always update serialized properties at start of OnInspectorGUI
		serializedObject.Update ();

		//start listening for changes in inspector values
		EditorGUI.BeginChangeCheck ();

		//reference to the slider
		VRBasics vrbascis = (VRBasics) target;

		EditorGUILayout.PropertyField (autoVRTypeProp, new GUIContent ("Auto VR Type"));


		if (!vrbascis.autoVRType) {
			EditorGUILayout.PropertyField (vrTypeProp, new GUIContent ("VR Type"));
		}

		EditorGUILayout.PropertyField (ignoreCollisionsLayerProp, new GUIContent ("Ignore Collisions"));
		EditorGUILayout.PropertyField (renderScaleProp, new GUIContent ("Render Scale"));

		//if there were any changes in inspector values
		if (EditorGUI.EndChangeCheck ()) {

			//apply changes to serialized properties
			serializedObject.ApplyModifiedProperties ();
		}
	}
}
