using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnityEventOnStart : MonoBehaviour
{
    [SerializeField]
    UnityEngine.Events.UnityEvent m_unityEvent;
    int m_waitForFramesBeforeCasting = 2;
    // Start is called before the first frame update
    void Start()
    {
        if(m_unityEvent != null)
            StartCoroutine(DelayedAction(m_waitForFramesBeforeCasting, ()=>m_unityEvent.Invoke()));
    }

    IEnumerator DelayedAction(int numFrames, System.Action act){
        for(int i = 0; i<numFrames; i++)
            yield return new WaitForEndOfFrame();
        act();
    }
}
