using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class SliderScrubCounter : MonoBehaviour
{
    /// <summary>
    /// this was necessary so other scripts know what is the expected completed value.
    /// </summary>
    public int maxScrubs = 4;
    public int scrubCount = 0;
    private Throwable m_Throwable;

    enum Vals{min, max}
    Vals lastVal;
    // Start is called before the first frame update
    void Start()
    {
        m_Throwable = GetComponentInChildren<Throwable>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnSliderMinValueReached(){
        lastVal = Vals.min;
    }

    public void OnSliderMaxValueReached(){
        //Debug.Log(lastVal);
        if(lastVal != Vals.max){
            scrubCount++;
            m_Throwable.ManuallyDetatchThisObject();
            gameObject.SetActive(false);
        }
        lastVal = Vals.max;
    }

    public void ResetScrubCount(){
        scrubCount = 0;
    }
}
