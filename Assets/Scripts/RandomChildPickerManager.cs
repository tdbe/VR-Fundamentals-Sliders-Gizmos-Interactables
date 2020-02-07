using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomChildPickerManager : MonoBehaviour
{
    [SerializeField]
    Transform m_outsideProblems;
    [SerializeField]
    Transform m_insideProblems;
    PickRandomChildrenToTurnOn[]  m_PickRandomChildrenToTurnOnArray_Outside;
    PickRandomChildrenToTurnOn[]  m_PickRandomChildrenToTurnOnArray_Inside;
    // Start is called before the first frame update
    void Start()
    {
        m_PickRandomChildrenToTurnOnArray_Outside = m_outsideProblems.GetComponentsInChildren<PickRandomChildrenToTurnOn>();
        m_PickRandomChildrenToTurnOnArray_Outside = m_insideProblems.GetComponentsInChildren<PickRandomChildrenToTurnOn>();
    }

    public void TurnAllChildrenOnOrOff(bool turn){
        foreach(PickRandomChildrenToTurnOn child in m_PickRandomChildrenToTurnOnArray_Outside){
            child.TurnAllChildrenOnOrOff(turn);
            Debug.Log("TurnAllChildrenOnOrOff Outside: "+turn+"; chid:"+child.transform.name);
        }
        foreach(PickRandomChildrenToTurnOn child in m_PickRandomChildrenToTurnOnArray_Inside){
            child.TurnAllChildrenOnOrOff(turn);
            Debug.Log("TurnAllChildrenOnOrOff Inside: "+turn+"; chid:"+child.transform.name);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
