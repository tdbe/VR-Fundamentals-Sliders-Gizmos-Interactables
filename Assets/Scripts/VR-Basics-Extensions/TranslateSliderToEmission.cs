using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TranslateSliderToEmission : MonoBehaviour
{
    
    public float maxValue = 30f;
    ParticleSystem m_ps;//TODO: have a mouse only dropdown here

    // Start is called before the first frame update
    void Start()
    {
        m_ps = GetComponent<ParticleSystem>();
    }

    // Update is called once per frame
    public void UpdateEmission(InteractableValue interactableValue)
    {
        if (interactableValue.sliderPosition > 0.0f && !m_ps.isPlaying)
        {
            m_ps.Play();
        }
        else if (interactableValue.sliderPosition == 0.0f && m_ps.isPlaying)
        {
            m_ps.Stop();
        }

        var emission = m_ps.emission;
        emission.rateOverTime = interactableValue.sliderPosition * maxValue;
    }


}
