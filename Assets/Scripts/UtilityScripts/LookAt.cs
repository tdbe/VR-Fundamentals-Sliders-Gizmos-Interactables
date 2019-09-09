using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAt : MonoBehaviour {
	[Header("Fields are autopopulated if left blank.")]
	[SerializeField]
	Transform[] m_objectsToRotate;

	[SerializeField]
	Transform m_lookAtTarget;

	// Use this for initialization
	void Start () {
		if(m_lookAtTarget == null){
			m_lookAtTarget = Camera.main.transform;
		}

		if(m_objectsToRotate == null || m_objectsToRotate.Length == 0){
			m_objectsToRotate = new Transform[1];
			m_objectsToRotate[0] = transform;
		}
	}
	
	// Update is called once per frame
	void Update () {
		for(int i = 0; i< m_objectsToRotate.Length; i++){
			m_objectsToRotate[i].LookAt(m_lookAtTarget);
		}
	}
}
