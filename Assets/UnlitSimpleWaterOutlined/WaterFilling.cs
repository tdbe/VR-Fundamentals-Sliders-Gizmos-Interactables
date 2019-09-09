using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.Events;

[ExecuteInEditMode]
public class WaterFilling : MonoBehaviour
{
    [SerializeField]
    const string _MatFadeProperty = "_Mask";

    [SerializeField]
    Vector3 m_meshLocalPosEmpty;
    [SerializeField]
    Vector3 m_meshLocalPosFull;
    [SerializeField]
    float m_MatFadePropEmpty;
    [SerializeField]
    float m_MatFadePropFull;
    
    [SerializeField]
    float m_autoFillTime = 2;
    [SerializeField]
    float m_FillTimeMod = 1;
    [SerializeField]
    float m_DrainTimeMod = 1;
    float m_SpillTimeMod = 1;
    
    float m_StartTime;
    [Range(0,1)]
    [SerializeField]
    public float m_slider = 1;
    float m_sliderLF = 1;
    #if UNITY_EDITOR
    [SerializeField]
    bool m_testAnimation;
    #endif
    Material m_mat;
    Coroutine m_corFill;
    Coroutine m_corDrain;

    [SerializeField]
    float m_angleBeforeSpill = 10;
    [SerializeField]
    float m_CurrentAngle;
    float angleLF;
    [SerializeField]
    float m_spillDrainSpeed = 10;

    [SerializeField]
    public bool isWaterContaminated = false;

    //public UnityEvent OnWaterFilledComplete

    enum FillState{
        IsNeutral = 0,
        IsFilling = 1,
        IsDraining = 2,
        IsBothFillingAndDraining = 3
        
    }
    FillState m_isFilling = FillState.IsNeutral;

    // Start is called before the first frame update
    void OnEnable()
    {
        m_mat = GetComponent<MeshRenderer>().sharedMaterial;
        //m_slider = 1;
    }

    void Start(){
        m_slider = 0;
    }

    public void ContaminantInWater(bool value){
        isWaterContaminated = value;
    }

    // Update is called once per frame
    void Update()
    {
        #if UNITY_EDITOR
            if(m_testAnimation){
                m_testAnimation = false;
                if(m_slider>0.5f)
                    DrainAuto();
                else{
                    FillUpAuto();
                }
            }
        #endif


        #region Make the water spill
        m_SpillTimeMod = Mathf.Abs(-Vector3.Dot(Vector3.up, transform.up)*m_spillDrainSpeed);
        m_CurrentAngle = Vector3.Angle(Vector3.up, transform.parent.up);
        if(m_CurrentAngle > m_angleBeforeSpill && angleLF <= m_angleBeforeSpill ||
            m_CurrentAngle > m_angleBeforeSpill && m_isFilling != FillState.IsDraining && m_isFilling != FillState.IsBothFillingAndDraining && m_slider !=0){
            DrainAuto(m_DrainTimeMod);
            Debug.Log("Starting to spill water.");
        }
        else if(m_CurrentAngle < m_angleBeforeSpill && angleLF >= m_angleBeforeSpill){
            StopDraining();
        }
        angleLF = m_CurrentAngle;
        #endregion

        if(m_sliderLF != m_slider)
            FillUp(m_slider);
        m_sliderLF = m_slider;
        

    }

    public void StopDraining(){
        if(m_corDrain!=null){
            if(m_isFilling == FillState.IsDraining){
                StopCoroutine(m_corDrain);
                m_isFilling = FillState.IsNeutral;
            }
            else if(m_isFilling == FillState.IsBothFillingAndDraining){
                StopCoroutine(m_corDrain);
                m_isFilling = FillState.IsFilling;
            }
        }
    }

    public void StopFilling(){
        if(m_corFill!=null){
            if(m_isFilling == FillState.IsFilling){
                StopCoroutine(m_corFill);
                m_isFilling = FillState.IsNeutral;
            }
            else if(m_isFilling == FillState.IsBothFillingAndDraining){
                StopCoroutine(m_corFill);
                m_isFilling = FillState.IsDraining;
            }
        }
    }

    public void StopFillingOrDraining(){
        if(m_corFill!=null){
            StopCoroutine(m_corFill);
            m_isFilling = m_isFilling == FillState.IsBothFillingAndDraining ? FillState.IsDraining : FillState.IsNeutral;
        }
        if(m_corDrain!=null){
            StopCoroutine(m_corDrain);
            m_isFilling = m_isFilling == FillState.IsBothFillingAndDraining ? FillState.IsFilling : FillState.IsNeutral;
        }
    }

    public void FillUpSpeedMod(InteractableValue interactableValue){
        m_FillTimeMod = interactableValue.sliderPosition;
    }

    public void DrainSpeedMod(InteractableValue interactableValue){
        m_DrainTimeMod = interactableValue.sliderPosition;
    }

    public void FillUpAuto(float speed = 1){
        m_FillTimeMod = speed;
        if(m_corFill!=null)
            StopCoroutine(m_corFill);
        m_isFilling = m_isFilling == FillState.IsDraining ? FillState.IsBothFillingAndDraining : FillState.IsFilling;
        m_corFill = StartCoroutine(AnimateWater(Time.time, m_autoFillTime, m_slider, FillState.IsFilling));
    }

    public void DrainAuto(float speed = 1){
        m_DrainTimeMod = speed;
        if(m_corDrain!=null)
            StopCoroutine(m_corDrain);
        m_isFilling = m_isFilling == FillState.IsFilling ? FillState.IsBothFillingAndDraining : FillState.IsDraining;
        m_corDrain = StartCoroutine(AnimateWater(Time.time, m_autoFillTime, m_slider, FillState.IsDraining));
    }

    IEnumerator AnimateWater(float startTime, float duration, float slider, FillState filling){
      
        //while(Time.time - startTime < duration*m_FillTimeMod){
        while(slider <= 1.0f && slider >= 0.0f){

            if(m_isFilling == FillState.IsFilling || m_isFilling == FillState.IsBothFillingAndDraining && filling == FillState.IsFilling)
                slider += Time.deltaTime/duration*m_FillTimeMod;
            if(m_isFilling == FillState.IsDraining || m_isFilling == FillState.IsBothFillingAndDraining && filling == FillState.IsDraining)
                slider -= Time.deltaTime/duration*Mathf.Max(m_DrainTimeMod,m_SpillTimeMod);
            slider = Mathf.Clamp01(slider);
            
            m_slider = slider;
            //FillUp(slider);

            yield return new WaitForEndOfFrame();
        }
        if(m_isFilling == FillState.IsFilling || m_isFilling == FillState.IsDraining)
            m_isFilling = FillState.IsNeutral;
        else if(m_isFilling == FillState.IsBothFillingAndDraining){
            if(filling == FillState.IsFilling)
                m_isFilling = FillState.IsDraining;
            else if(filling == FillState.IsDraining)
                m_isFilling = FillState.IsFilling;
            else
                Debug.LogError("FillState error. This is undefined behaviour.");
        }
            
    }

    public void FillUp(float slider){
        transform.localPosition = Vector3.Lerp(m_meshLocalPosEmpty, m_meshLocalPosFull, slider);
        float matProp = Mathf.Lerp(m_MatFadePropEmpty, m_MatFadePropFull, slider);
        m_mat.SetFloat(_MatFadeProperty, matProp);
    }
}
