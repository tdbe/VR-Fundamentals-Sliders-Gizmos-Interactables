using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TranslateSliderToBlendshape : MonoBehaviour {
    
    private SkinnedMeshRenderer m_Renderer;
    
    private void Start () {
        m_Renderer = GetComponent<SkinnedMeshRenderer>();
    }

    public void UpdateAnimation (InteractableValue interactableValue) {
        m_Renderer.SetBlendShapeWeight(0, interactableValue.sliderPosition * 100f);
    }
}