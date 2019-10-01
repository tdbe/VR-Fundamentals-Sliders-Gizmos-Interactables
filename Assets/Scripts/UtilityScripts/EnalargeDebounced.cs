using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnalargeDebounced : MonoBehaviour
{
    [SerializeField]
    float m_waitBeforeEnlarge = 1.2f;
    [SerializeField]
    float m_enlargeAnimDuration = 1f;
    [SerializeField]
    AnimationCurve m_enlargeAnimCurve;
    [SerializeField]
    float m_shrinkAnimDuration = .5f;

    [SerializeField]
    Vector3 m_enlargedSize = Vector3.one;
    Vector3 m_originalSize;

    Coroutine m_corE;
    Coroutine m_corS;


    // Start is called before the first frame update
    void OnEnable()
    {
        m_originalSize = transform.localScale;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void EnlargeDebounced(){
        if(m_corE!=null)
            StopCoroutine(m_corE);
        m_corE = StartCoroutine(EnlargeDebouncedCor());
    }

    IEnumerator EnlargeDebouncedCor(){
        //yield return new WaitForEndOfFrame();
        yield return new WaitForSeconds(m_waitBeforeEnlarge);
        if(m_corS!=null){
            StopCoroutine(m_corS);
        }
   

        float startTime = Time.time;
        float currentTime = 0;
        m_originalSize = transform.localScale;
        do{
            currentTime = Time.time-startTime;
            float scrub = currentTime/m_enlargeAnimDuration;
            transform.localScale = Vector3.Lerp(m_originalSize, m_enlargedSize, m_enlargeAnimCurve.Evaluate(scrub));
            yield return new WaitForEndOfFrame();
        }while(currentTime<=m_enlargeAnimDuration);
        transform.localScale = m_enlargedSize;
    }

    IEnumerator ShrinkCor(){

        float startTime = Time.time;
        float currentTime = 0;
        Vector3 currentSize = transform.localScale;
        do{
            currentTime = Time.time-startTime;
            float scrub = currentTime/m_shrinkAnimDuration;
            transform.localScale = Vector3.Lerp(currentSize, m_originalSize, m_enlargeAnimCurve.Evaluate(scrub));
            yield return new WaitForEndOfFrame();
        }while(currentTime<=m_shrinkAnimDuration);
        transform.localScale = m_originalSize;

    }

    public void ShrinkBackOrCancelEnlarge(){
        if(m_corE!=null){
            StopCoroutine(m_corE);
        }
        if(Vector3.Distance(transform.localScale,m_originalSize)>0.01f)
            m_corS = StartCoroutine(ShrinkCor());
        else
            transform.localScale = m_originalSize;
    }
}
