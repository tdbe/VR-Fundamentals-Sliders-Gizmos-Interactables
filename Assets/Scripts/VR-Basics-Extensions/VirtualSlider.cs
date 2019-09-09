using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[ExecuteInEditMode]
public class VirtualSlider : InteractableValue
{
    [SerializeField]
    float m_sliderSpeed = 1;
    float prevPos;

    [System.Serializable]
    public class UnityInteractableValueEvent : UnityEvent<InteractableValue>
    {
    }
    [SerializeField]
    UnityInteractableValueEvent onSliderPositionUpdate;

    [SerializeField]
    UnityEvent onSliderBinaryOn;
    [SerializeField]
    UnityEvent onSliderBinaryOff;
    [SerializeField]
    UnityEvent onSliderOnMax;
    [SerializeField]
    UnityEvent onSliderOn_25Percent;
    [SerializeField]
    UnityEvent onSliderOn_75Percent;

    Coroutine m_coroutine;

    float m_minMechanicalSliderValue = 0.05f;
    float m_maxMechanicalSliderValue = 0.95f;

    #if UNITY_EDITOR
    public bool m_testSlider;
    #endif

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        #if UNITY_EDITOR
            if(m_testSlider){
                m_testSlider = false;
                MoveSliderAuto();
            }
        #endif
        
        if (prevPos!=sliderPosition){
            if(onSliderPositionUpdate!=null)
                onSliderPositionUpdate.Invoke(this);

            if(sliderPosition >=.25f && prevPos<.25f && onSliderOn_25Percent!=null)
                onSliderOn_25Percent.Invoke();
            if(sliderPosition >=.75f && prevPos<.75f && onSliderOn_75Percent!=null)
                onSliderOn_75Percent.Invoke();

            if(onSliderBinaryOn!=null && sliderPosition>m_minMechanicalSliderValue && prevPos <= m_minMechanicalSliderValue){
                onSliderBinaryOn.Invoke();
            }
            else if(onSliderOnMax!=null && sliderPosition>m_maxMechanicalSliderValue && prevPos <= m_maxMechanicalSliderValue){
                onSliderOnMax.Invoke();
                sliderPosition = 1;
            }
            else if(onSliderBinaryOff!=null && sliderPosition<=m_minMechanicalSliderValue && prevPos > m_minMechanicalSliderValue){
                onSliderBinaryOff.Invoke();
                sliderPosition = 0;
            }
                
        }
        prevPos = sliderPosition;
    }


    public override void ResetInteractable(){
       
        sliderPosition = 0;
    }

    public void MoveSliderAuto(float speed = 1){
        if(m_coroutine!=null){
            StopCoroutine(m_coroutine);
        }
        float spd = speed*m_sliderSpeed;

        if(sliderPosition == 1.0f && spd >0 ){
            spd *=-1;
        }
        m_coroutine = StartCoroutine(AnimateSlider(Time.time, spd, sliderPosition));
    }


    IEnumerator AnimateSlider(float startTime, float duration, float slider){
      
        //while(Time.time - startTime < duration*m_FillTimeMod){
        while(duration != 0.0f && slider <= 1.0f && slider >= 0.0f){

            if(duration > 0)
                slider += Time.deltaTime/duration;
            if(duration < 0)
                slider -= Time.deltaTime/-duration;
            slider = Mathf.Clamp01(slider);
            
            sliderPosition = slider;
            if(slider ==1.0f || slider==0f)
                break;
            //FillUp(slider);
        
            yield return new WaitForEndOfFrame();
        }
        
            
    }

}
