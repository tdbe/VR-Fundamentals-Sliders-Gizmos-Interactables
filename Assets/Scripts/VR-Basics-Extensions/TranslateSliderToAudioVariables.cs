using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TranslateSliderToAudioVariables : MonoBehaviour {
    //public float weight = 1f;
    private AudioSource m_AudioSource;
    void Start () {
        m_AudioSource = GetComponent<AudioSource> ();
    }

    private void OnEnable () {

    }

    // Update is called once per frame
    public void UpdateAudioVariables (InteractableValue interactableValue) {
        if (m_AudioSource != null) {
            m_AudioSource.pitch = interactableValue.sliderPosition;
            m_AudioSource.volume = interactableValue.sliderPosition;
        }
    }
}