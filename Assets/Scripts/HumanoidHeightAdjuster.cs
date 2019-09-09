using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanoidHeightAdjuster : MonoBehaviour {

	[SerializeField]
	float m_initialEyeHeight = 1.727f;
	[SerializeField]
	Transform m_humanoidRoot;
	Vector3 m_origHumanoidScale;
	Vector3 m_origEyeScale;
	[SerializeField]
	Transform m_camEyeTransform;

	[SerializeField]
	bool m_alsoScaleEyes;

	[SerializeField]
	float m_extraScale = 1.15f;

	// Use this for initialization
	void Start () {
		if(m_humanoidRoot == null)	
			m_humanoidRoot = transform;
		if(m_camEyeTransform == null)
			m_camEyeTransform = Camera.main.transform;
		m_origHumanoidScale = m_humanoidRoot.localScale;
		m_origEyeScale = m_camEyeTransform.localScale;

		//CalculateHeight();
	}
	
	// Update is called once per frame
	void Update () {
		if(InputManager.Instance.setHeight){
			CalculateHeight();
		}
	}

	public void CalculateHeight(){
		float currentHeight = 0;//m_initialEyeHeight;
		RaycastHit hit;
		Vector3 rayOrigin = m_camEyeTransform.position;// + m_humanoidRoot.up*0.3f;
		Vector3 rayDir = -m_humanoidRoot.up;
		//Debug.DrawRay(rayOrigin, rayDir, Color.red,0.5f);
		Debug.DrawLine(rayOrigin+Vector3.right*1.1f, rayOrigin + rayDir*50, Color.green, 0.4f);
		if (Physics.Raycast(rayOrigin, rayDir, out hit, 50, ~(1<<8))){
			//if the object we hit is a child of this game object
			currentHeight = Vector3.Distance(m_camEyeTransform.position, hit.point);
			Debug.DrawLine(rayOrigin+Vector3.right, hit.point, Color.red, 0.4f);
		}
		else
			currentHeight = Vector3.Distance(m_camEyeTransform.position, m_humanoidRoot.position);

		currentHeight *= m_extraScale;

		if(currentHeight>3.5f)
			m_humanoidRoot.GetChild(0).gameObject.SetActive(false);
		else
			m_humanoidRoot.GetChild(0).gameObject.SetActive(true);

		float delta = currentHeight / m_initialEyeHeight;
		m_humanoidRoot.localScale = m_origHumanoidScale * delta;

		if(m_alsoScaleEyes)
			m_camEyeTransform.localScale = m_origEyeScale * delta;
	}
}
