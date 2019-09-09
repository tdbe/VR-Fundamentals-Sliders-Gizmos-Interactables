/***********************************************************************************
\file		SteamVRMainHandManager.cs (version 1.0)
\brief		
\long		
\copyright Copyright 2019 Khora VR, LLC All Rights reserved.
************************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class SteamVRMainHandManager : MonoBehaviour {

	//==============================================================================
	// Fields
	//==============================================================================
	SteamVR_Input_Sources m_steamVrInputSourceMainHand = SteamVR_Input_Sources.Any;
	[Tooltip("The left controller's transform")]
	[SerializeField] private Transform m_LeftHandTransform;
	[Tooltip("The right controller's transform")]
	[SerializeField] private Transform m_RightHandTransform;
	[Tooltip("The object that should follow either hands")]
	[SerializeField] private Transform m_MainHandTransformProxy;
	private int m_lastHandWithPressedTrigger = -1;

	//==============================================================================
	// Mutator
	//==============================================================================
	public SteamVR_Input_Sources steamVrInputSourceMainHand { get { return m_steamVrInputSourceMainHand; } }
	public Transform GetLeftHandTransform { get { return m_LeftHandTransform; } }
	public Transform GetRightHandTransform { get { return m_RightHandTransform; } }
	public Transform GetMainHandTransform { get { return m_MainHandTransformProxy; } }

	//==============================================================================
	// Monobehavior
	//==============================================================================
	void Update() {
		SetMainHand();
	}

	//==============================================================================
	// Private
	//==============================================================================
	private void SetMainHand() {
		bool isRightActive = false;
		bool isLeftActive = false;

		// SteamVR_Action_Pose poseAction = SteamVR_Input._default.inActions.Pose;
		// if (poseAction.GetActive(SteamVR_Input_Sources.LeftHand)) {
		// 	if (poseAction.GetDeviceIsConnected(SteamVR_Input_Sources.LeftHand)) {
		// 		isLeftActive = true;
		// 	}
		// }

		// if (poseAction.GetActive(SteamVR_Input_Sources.RightHand)) {
			// if (poseAction.GetDeviceIsConnected(SteamVR_Input_Sources.RightHand)) {
			// 	isRightActive = true;
			// }
		// }

		if (isLeftActive && isRightActive) {
			/*
			if (SteamVR_Input._default.inActions.InteractUI.GetStateDown(SteamVR_Input_Sources.LeftHand)) {
				m_lastHandWithPressedTrigger = 0;
			}
			if (SteamVR_Input._default.inActions.InteractUI.GetStateDown(SteamVR_Input_Sources.RightHand)) { //It currently prioritises right if pressed at the same time
				m_lastHandWithPressedTrigger = 1;
			}
			*/
			isRightActive = false;
			isLeftActive = false;
			if (m_lastHandWithPressedTrigger == 0) {
				isLeftActive = true;
			} else if (m_lastHandWithPressedTrigger == 1) {
				isRightActive = true;
			} else {
				isRightActive = true;
			}
		} else {
			m_lastHandWithPressedTrigger = -1;
		}

		if (isRightActive) {
			m_MainHandTransformProxy.transform.parent = m_RightHandTransform;
			m_MainHandTransformProxy.transform.position = m_RightHandTransform.position;
			m_MainHandTransformProxy.transform.rotation = m_RightHandTransform.rotation;
			m_steamVrInputSourceMainHand = SteamVR_Input_Sources.RightHand;
		} else if (isLeftActive) {
			m_MainHandTransformProxy.transform.parent = m_LeftHandTransform;
			m_MainHandTransformProxy.transform.position = m_LeftHandTransform.position;
			m_MainHandTransformProxy.transform.rotation = m_LeftHandTransform.rotation;
			m_steamVrInputSourceMainHand = SteamVR_Input_Sources.LeftHand;
		} else {
			if (!(Time.frameCount % 30 == 0))
				return;
			Debug.Log("BOTH HANDS ARE INACTIVE");
		}
	}
}