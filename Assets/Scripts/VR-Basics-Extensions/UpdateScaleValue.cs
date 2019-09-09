using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateScaleValue : MonoBehaviour
{
    public float minValue;
    public float maxValue;
    public Vector3 scaleDirection = Vector3.forward;

    Vector3 CachedMinVal;
    Vector3 CachedMaxVal;
    Vector3 m_origScale;
    //public SkinnedMeshRenderer skinnedMeshRenderer;

    // Start is called before the first frame update
    void Awake()
    {
        m_origScale = transform.localScale;

        CachedMinVal = (m_origScale-scaleDirection) + scaleDirection*minValue;
        CachedMaxVal = (m_origScale-scaleDirection) + scaleDirection*maxValue;
    }

    // Update is called once per frame
    public void UpdateScale(InteractableValue interactableValue)
    {
        Vector3 scale = Vector3.Lerp(CachedMinVal, CachedMaxVal, interactableValue.sliderPosition);
        transform.localScale = scale;
    }
}
