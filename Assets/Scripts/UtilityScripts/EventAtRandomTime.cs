using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventAtRandomTime : MonoBehaviour
{
    public float delayedStart = 7*60;
    public int triggerFiniteTimes = -1;
    public int timesTriggered;
    public Vector2 m_WaitAtLeastAndAtMostSeconds = new Vector2(30, 120);
    public UnityEvent RandomTimeEvent;

    //public float lastCastTime;
    public float nextCastTime;

    // Start is called before the first frame update
    void Start()
    {
        nextCastTime = GetNextCastTime();
    }

    // Update is called once per frame
    void Update()
    {
        if(delayedStart >=0 && Time.time > delayedStart){
            if(Time.time >= nextCastTime && (triggerFiniteTimes<0 || timesTriggered < triggerFiniteTimes)){
                if(RandomTimeEvent!=null){
                    RandomTimeEvent.Invoke();
                }
                timesTriggered++;
                //lastCastTime = Time.time;
                nextCastTime = GetNextCastTime();
            }
        }
    }

    public void ManuallyCastNow(){
        delayedStart = 0;
        nextCastTime = GetNextCastTime();
    }

    float GetNextCastTime(){
        return Mathf.Max(Time.time, delayedStart) + Random.Range(m_WaitAtLeastAndAtMostSeconds.x, m_WaitAtLeastAndAtMostSeconds.y);
    }
}
