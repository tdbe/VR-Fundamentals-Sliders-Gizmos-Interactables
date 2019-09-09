using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeMaterialFromSlider : MonoBehaviour {
    [SerializeField]
    Material m_materialToUpdate;
    [SerializeField]
    string m_ShaderPropertyToChange = "_DirtSlider";
    SliderScrubCounter m_sliderScrubCounter;

    // Start is called before the first frame update
    void Awake () {
        m_sliderScrubCounter = GetComponent<SliderScrubCounter> ();
    }

    public void OnSliderValueUpdate (InteractableValue interactableValue) {
        float globalSlider = interactableValue.sliderPosition;
        if (m_sliderScrubCounter != null) {
            float val = (globalSlider * m_sliderScrubCounter.scrubCount + 1) / m_sliderScrubCounter.maxScrubs;
            m_materialToUpdate.SetFloat (m_ShaderPropertyToChange, val);
            //Debug.Log("Setting skin material slider "+m_ShaderPropertyToChange+", to: "+
            //"globalSlider{"+globalSlider+"} * m_sliderScrubCounter.scrubCount{"+m_sliderScrubCounter.scrubCount+1+"}) / m_sliderScrubCounter.maxScrubs{"+m_sliderScrubCounter.maxScrubs+"} = "+val);            
        } else {
            m_materialToUpdate.SetFloat (m_ShaderPropertyToChange, globalSlider);
        }
    }

    public void SetClean () {
        m_materialToUpdate.SetFloat (m_ShaderPropertyToChange, 1);
    }

}