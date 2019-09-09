using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class AnimateWaterMaterial : MonoBehaviour {
    [SerializeField]
    Material m_waterMat;
    [SerializeField]
    Vector2 m_direction;
    readonly string _MainTex = "_MainTex";
    readonly string _SunDir = "_SunDir";
    readonly string _SpecPos = "_SpecPos";
    readonly string _CamDir = "_CamDir";
    
    Vector2 textureOffset;

    [SerializeField]
    float m_rippleWobbleSpeed = 10;

    [SerializeField]
    Transform m_sunDirection;

    [SerializeField]
    Transform m_camera;

    Vector3 m_UpPos;
    

    void OnEnable ()
    {
        m_UpPos = Vector3.up;
        m_UpPos.y = -9999;

        m_waterMat.SetTextureOffset(_MainTex, Vector2.zero);
        textureOffset = m_waterMat.GetTextureOffset(_MainTex);
        
#if UNITY_EDITOR
        if(Application.isPlaying){
            m_waterMat.DisableKeyword("PC_EDITOR");
            m_waterMat.EnableKeyword("MOBILE_VR");
            //Debug.Log("Is Playing: m_waterMat.IsKeywordEnabled(PC_EDITOR): "+m_waterMat.IsKeywordEnabled("PC_EDITOR")+"; m_waterMat.IsKeywordEnabled(MOBILE_VR): "+m_waterMat.IsKeywordEnabled("MOBILE_VR"));
        }
        else{
            m_waterMat.DisableKeyword("MOBILE_VR");
            m_waterMat.EnableKeyword("PC_EDITOR");
            //Debug.Log("Is Not Playing, Editor: m_waterMat.IsKeywordEnabled(PC_EDITOR): "+m_waterMat.IsKeywordEnabled("PC_EDITOR")+"; m_waterMat.IsKeywordEnabled(MOBILE_VR): "+m_waterMat.IsKeywordEnabled("MOBILE_VR"));

        }
#else
        m_waterMat.DisableKeyword("PC_EDITOR");
        m_waterMat.EnableKeyword("MOBILE_VR");
#endif

    }

    // Update is called once per frame
    void Update () {
        textureOffset += m_direction * Time.deltaTime * m_rippleWobbleSpeed;
        m_waterMat.SetTextureOffset(_MainTex, textureOffset);

        if (m_sunDirection != null && m_sunDirection.gameObject.activeInHierarchy)
        {
            m_waterMat.SetVector(_SunDir, m_sunDirection.forward);
            m_waterMat.SetVector(_SpecPos, m_sunDirection.position);
            m_waterMat.SetVector(_CamDir, m_camera.forward);
        }
        
        transform.LookAt(m_UpPos);
    }
}
