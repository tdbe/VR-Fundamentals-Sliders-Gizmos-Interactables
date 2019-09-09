using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class TranslateSliderToRotation : MonoBehaviour {
    public Vector3 rotationDirection = Vector3.up;
    public float maxAngle = -78.6f;
    public float weight = 1f;
    public bool m_UseLocalRotation = false;
    private Quaternion m_OriginalRotation;
    // Start is called before the first frame update
    void Start () {
    }

    private void OnEnable() {
        if (m_UseLocalRotation)
            m_OriginalRotation = transform.localRotation;
        else
            m_OriginalRotation = transform.rotation;
    }

    // Update is called once per frame
    public void UpdateRotation (InteractableValue interactableValue) {
        /*
        Vector3 rot = transform.rotation.eulerAngles;
        rot.x *= rotationDirection.x;
        rot.y *= rotationDirection.y;
        rot.z *= rotationDirection.z;*/
        Vector3 newRot = rotationDirection * interactableValue.sliderPosition * maxAngle * weight;
        if (m_UseLocalRotation)
        
            transform.localRotation = m_OriginalRotation * Quaternion.Euler (newRot);
        else
            transform.rotation = m_OriginalRotation * Quaternion.Euler (newRot);
    }
}