using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TranslateSliderToAnimation : MonoBehaviour {
    [SerializeField]
    float value;
    private Animator m_Animator;
    private void Start () {
        m_Animator = GetComponent<Animator> ();
    }
    public void UpdateAnimation (InteractableValue interactableValue) {
        value = interactableValue.sliderPosition;
        m_Animator.Play ("duvetMove", 0, interactableValue.sliderPosition);
    }
}