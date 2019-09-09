using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextCanvasOptimizer : MonoBehaviour
{
    [Header("Finds and disables the following components after they've fit the text size")]
    [SerializeField]
    int m_disableAfterNumberOfFrames = 2;
    [SerializeField]
    CanvasScaler m_canvasScaler;
    [SerializeField]
    bool m_DisableCanvasScalerIfAvailable = true;

    [SerializeField]
    GraphicRaycaster m_GraphicRaycaster;
    [SerializeField]
    bool m_DisableGraphicRaycasterIfAvailable = true;

    [SerializeField]
    ContentSizeFitter m_ContentSizeFitter;
    [SerializeField]
    bool m_DisableContentSizeFitterIfAvailable = true;

    [SerializeField]
    HorizontalLayoutGroup m_HorizontalLayoutGroup;
    [SerializeField]
    bool m_DisableHorizontalLayoutGroupIfAvailable = true;

    [SerializeField]
    Text m_textToMonitor;
    float m_maxTextLen = 0;


    // Start is called before the first frame update
    void Awake()
    {
        m_canvasScaler = GetComponentInChildren<CanvasScaler>();
        m_GraphicRaycaster = GetComponentInChildren<GraphicRaycaster>();
        m_ContentSizeFitter = GetComponentInChildren<ContentSizeFitter>();
        m_HorizontalLayoutGroup = GetComponentInChildren<HorizontalLayoutGroup>();
        m_textToMonitor = GetComponentInChildren<Text>();
        StartCoroutine(EnableDisableObjects(false, m_disableAfterNumberOfFrames));
    }

    void Update(){
        if(m_textToMonitor!=null && m_textToMonitor.text.Length > m_maxTextLen){
            m_maxTextLen = m_textToMonitor.text.Length;
            StartCoroutine(EnableDisableObjects(true, 0));
            StartCoroutine(EnableDisableObjects(false, 2));
        }

    }

    IEnumerator EnableDisableObjects(bool enable, float afterFrames){
        for(int i = 0; i<afterFrames; i++)
            yield return new WaitForEndOfFrame();

        if(m_canvasScaler!=null && m_DisableCanvasScalerIfAvailable)
            m_canvasScaler.enabled = enable;
        if(m_GraphicRaycaster!=null && m_DisableGraphicRaycasterIfAvailable)
            m_GraphicRaycaster.enabled = enable;
        if(m_ContentSizeFitter!=null && m_DisableContentSizeFitterIfAvailable)
            m_ContentSizeFitter.enabled = enable;
        if(m_HorizontalLayoutGroup!=null && m_DisableHorizontalLayoutGroupIfAvailable)
            m_HorizontalLayoutGroup.enabled = enable;
    }


}
