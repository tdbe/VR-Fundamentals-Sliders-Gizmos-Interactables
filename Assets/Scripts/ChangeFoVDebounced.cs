using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeFoVDebounced : MonoBehaviour
{
    [SerializeField]
    Camera m_camToChange;
    [SerializeField]
    float m_waitBeforeFOVChange = 1.2f;
    [SerializeField]
    float m_FOVChangeAnimDuration = 1f;
    [SerializeField]
    AnimationCurve m_FOVChangeAnimCurve;
    [SerializeField]
    float m_shrinkAnimDuration = .5f;

    [SerializeField]
    float m_FOVChangedSize = 60;
    float m_originalSize;

    Coroutine m_corE;
    Coroutine m_corS;


    // Start is called before the first frame update
    void OnEnable()
    {
        m_originalSize = m_camToChange.fieldOfView;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void FOVChangeDebounced(){
        if(m_corE!=null)
            StopCoroutine(m_corE);
        m_corE = StartCoroutine(FOVChangeDebouncedCor());
    }

    IEnumerator FOVChangeDebouncedCor(){
        //yield return new WaitForEndOfFrame();
        yield return new WaitForSeconds(m_waitBeforeFOVChange);
        if(m_corS!=null){
            StopCoroutine(m_corS);
        }
   

        float startTime = Time.time;
        float currentTime = 0;
        m_originalSize = m_camToChange.fieldOfView;
        do{
            currentTime = Time.time-startTime;
            float scrub = currentTime/m_FOVChangeAnimDuration;
            m_camToChange.fieldOfView = Mathf.Lerp(m_originalSize, m_FOVChangedSize, m_FOVChangeAnimCurve.Evaluate(scrub));
            yield return new WaitForEndOfFrame();
        }while(currentTime<=m_FOVChangeAnimDuration);
        m_camToChange.fieldOfView = m_FOVChangedSize;
    }

    IEnumerator ShrinkCor(){

        float startTime = Time.time;
        float currentTime = 0;
        float currentSize = m_camToChange.fieldOfView;
        do{
            currentTime = Time.time-startTime;
            float scrub = currentTime/m_shrinkAnimDuration;
            m_camToChange.fieldOfView = Mathf.Lerp(currentSize, m_originalSize, m_FOVChangeAnimCurve.Evaluate(scrub));
            yield return new WaitForEndOfFrame();
        }while(currentTime<=m_shrinkAnimDuration);
        m_camToChange.fieldOfView = m_originalSize;

    }

    public void ShrinkBackOrCancelFOVChange(){
        if(m_corE!=null){
            StopCoroutine(m_corE);
        }
        if(Mathf.Abs(m_camToChange.fieldOfView-m_originalSize)>0.01f)
            m_corS = StartCoroutine(ShrinkCor());
        else
            m_camToChange.fieldOfView = m_originalSize;
    }
}
