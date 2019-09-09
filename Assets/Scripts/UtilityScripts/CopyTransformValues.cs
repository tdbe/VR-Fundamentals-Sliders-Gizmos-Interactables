using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class CopyTransformValues : MonoBehaviour {

	[SerializeField]
	bool m_alwaysUpdateRotation = true;
	[SerializeField]
	bool m_alwaysUpdatePosition = false;
	[SerializeField]
	Vector3 updatePositionDirectionMask = Vector3.one;
	[SerializeField]
	Vector3 updatePositionExtraAdd = Vector3.zero;
	[SerializeField]
	bool m_alwaysUpdateScale = false;
	
	[SerializeField]
	Transform target;
	[SerializeField]
	bool m_copyFromFirstChildInstead;

	[SerializeField]
	bool m_forceUpdateOnPreCull = false;



	public void CopyPosition(Transform target){
		transform.position = target.position;
	}

	public void CopyRotation(Transform target){
		transform.rotation = target.rotation;
	}

	public void CopyScale(Transform target){
		transform.localScale = target.localScale;
	}

	public void CopyPositionAndRotation(Transform target){
		transform.position = target.position;
		transform.rotation = target.rotation;
	}

	public void CopyPositionAndRotationAndScale(Transform target){
		transform.position = target.position;
		transform.rotation = target.rotation;
		transform.localScale = target.localScale;		
	}

	void Awake(){
		if(target == null) return;

		if(m_copyFromFirstChildInstead)
			target = transform.GetChild(0);

		if(m_alwaysUpdateRotation)
			transform.rotation = target.rotation;
		if(m_alwaysUpdatePosition)
			UpdatePos();
		if(m_alwaysUpdateScale)
			transform.localScale = target.localScale;
	}

	void Start(){
		if(target == null) return;

		if(m_alwaysUpdateRotation)
			transform.rotation = target.rotation;
		if(m_alwaysUpdatePosition)
			UpdatePos();
		if(m_alwaysUpdateScale)
			transform.localScale = target.localScale;
	}

	void UpdatePos(){
		Vector3 objPos = transform.position;
		Vector3 targetPos = target.position + updatePositionExtraAdd;
		Vector3 mask = updatePositionDirectionMask;//transform.InverseTransformVector(updatePositionDirectionMask);
		
		objPos.x = Mathf.Lerp(objPos.x, targetPos.x, updatePositionDirectionMask.x);
		objPos.y = Mathf.Lerp(objPos.y, targetPos.y, updatePositionDirectionMask.y);
		objPos.z = Mathf.Lerp(objPos.z, targetPos.z, updatePositionDirectionMask.z);
		
		transform.position = objPos;
	}

	void Update(){
		if(target == null) return;

		if(!m_forceUpdateOnPreCull){
			if(m_alwaysUpdateRotation)
				transform.rotation = target.rotation;
			if(m_alwaysUpdatePosition)
				UpdatePos();
			if(m_alwaysUpdateScale)
				transform.localScale = target.localScale;
		}
	}

	void OnPreCull(){
		if(target == null) return;

		if(m_forceUpdateOnPreCull){
			if(m_alwaysUpdateRotation)
				transform.rotation = target.rotation;
			if(m_alwaysUpdatePosition)
				UpdatePos();
			if(m_alwaysUpdateScale)
				transform.localScale = target.localScale;
		}	
	}

/*
	public void ResetRigidbodyConstraints(){

	}

	public void RestoreRigidbodyConstraints(){

	}
	*/
}
