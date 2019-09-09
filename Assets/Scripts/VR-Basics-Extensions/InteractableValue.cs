using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InteractableValue : MonoBehaviour
{
    #region slider values
    [Range(0,1)]
    public float sliderPosition;
    [Tooltip("This slider can be moved from current segment to the next, but never back to a previous segment. Useful for washing motions. You can ResetInteractable() to get it back to 0.")]    
    public bool forwardOnlySlider;
    #endregion
    

    public abstract void ResetInteractable();
}
