using UnityEditor;
using UnityEngine;
using System.Collections;

[CustomEditor(typeof(VRBasics_ConnectorChild))]
[CanEditMultipleObjects]
public class VRBasics_ConnectorChildEditor : Editor {

	SerializedProperty typeProp;
	SerializedProperty coupleIDProp;
	SerializedProperty isConnectedProp;
	SerializedProperty attachAudioProp;
	SerializedProperty detachAudioProp;

	void OnEnable(){
		//set up serialized properties
		typeProp = serializedObject.FindProperty ("type");
		coupleIDProp = serializedObject.FindProperty ("coupleID");
		isConnectedProp = serializedObject.FindProperty ("isConnected");
		attachAudioProp = serializedObject.FindProperty ("attachAudio");
		detachAudioProp = serializedObject.FindProperty ("detachAudio");
	}

	public override void OnInspectorGUI ()
	{
		//reference to the class of object
		VRBasics_ConnectorChild connector = (VRBasics_ConnectorChild) target;

		//always update serialized properties at start of OnInspectorGUI
		serializedObject.Update ();

		//start listening for changes in inspector values
		EditorGUI.BeginChangeCheck ();

		//display serialized properties in inspector
		EditorGUILayout.PropertyField (typeProp,  new GUIContent ("Type"));
		EditorGUILayout.PropertyField (coupleIDProp,  new GUIContent ("Couple ID"));
		EditorGUILayout.PropertyField (isConnectedProp,  new GUIContent ("Is Connected"));

		//these properties are controlled by a male connector
		if (connector.type == VRBasics_ConnectorChild.connectorType.male) {
			EditorGUILayout.PropertyField (attachAudioProp,  new GUIContent ("Attach Audio"));
			EditorGUILayout.PropertyField (detachAudioProp,  new GUIContent ("Detach Audio"));
		}

		//if there were any changes in inspector values
		if (EditorGUI.EndChangeCheck ()) {

			//apply changes to serialized properties
			serializedObject.ApplyModifiedProperties ();

			if (Application.isPlaying) {
				connector.Detach ();
			}
		}
	}

	void OnSceneGUI() {

		//reference to the class of object used to display gizmo
		VRBasics_ConnectorChild connector = (VRBasics_ConnectorChild) target;

		//DRAW GIZMO
		connector.DrawGizmo();
	}
}
