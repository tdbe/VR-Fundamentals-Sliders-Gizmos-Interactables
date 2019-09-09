using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateBlendShapeValue : MonoBehaviour
{
    

    
    public SkinnedMeshRenderer skinnedMeshRenderer;

    // Start is called before the first frame update
    void Awake()
    {
        if(skinnedMeshRenderer == null)
            skinnedMeshRenderer = GetComponent<SkinnedMeshRenderer>();
    }

    // Update is called once per frame
    public void UpdateBlendWeight(InteractableValue interactableValue)
    {
        
        skinnedMeshRenderer.SetBlendShapeWeight(0,interactableValue.sliderPosition*100);
    }
}
