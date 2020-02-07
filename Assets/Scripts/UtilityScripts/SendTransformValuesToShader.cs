using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SendTransformValuesToShader : MonoBehaviour
{
    [SerializeField]
    Transform m_targetTransform;
    [SerializeField]
    MeshRenderer m_targetRenderer;
    Material m_mat;
    [SerializeField]
    float m_scaleScale = 20.57f;
    

    // Start is called before the first frame update
    void Start()
    {

        m_mat = m_targetRenderer.GetComponent<MeshRenderer>().sharedMaterial;
        if(m_mat==null)
            m_mat = GetComponent<MeshRenderer>().sharedMaterial;
        
    }

    // Update is called once per frame
    void Update()
    {
        m_mat.SetVector("_Scale", m_targetTransform.localScale*m_scaleScale);
    }
}
