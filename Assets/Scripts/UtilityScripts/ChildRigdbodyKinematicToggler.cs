using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChildRigdbodyKinematicToggler : MonoBehaviour {

	//
	[Header("Maintains all child rigidbodies' kinematic bool to opposite of this rigidbody's kinematic bool.", order=0)]
	[Space(10)]	
	[Header("Unless those children were already set to kinematic, in which case they're left alone.", order=1)]
	[Space(20)]
	List<Rigidbody> m_childRigidbodies;
	Rigidbody m_rigidbody;
	bool m_kinematicLastCheck;

	// Use this for initialization
	void Start () {
		m_rigidbody = GetComponent<Rigidbody>();
		m_kinematicLastCheck = m_rigidbody.isKinematic;
		m_childRigidbodies = new List<Rigidbody>();
		Rigidbody[] temp = gameObject.GetComponentsInChildren<Rigidbody>();
		for(int c = 0; c< temp.Length; c++){
			if(!temp[c].isKinematic)
				m_childRigidbodies.Add(temp[c]);
		}


		for(int i=0; i < m_childRigidbodies.Count; i++){
			m_childRigidbodies[i].isKinematic = !m_rigidbody.isKinematic;	
		}
	}
	
	// Update is called once per frame
	void Update () {
		if(m_rigidbody.isKinematic != m_kinematicLastCheck)
			for(int i=0; i < m_childRigidbodies.Count; i++){
				m_childRigidbodies[i].isKinematic = !m_rigidbody.isKinematic;	
		}
	}
}
