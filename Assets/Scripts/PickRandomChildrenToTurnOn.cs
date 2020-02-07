using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickRandomChildrenToTurnOn : MonoBehaviour
{
    [SerializeField]
    int m_turnOnBetweenThisManyChildren = 1;
    [SerializeField]
    int m_andThisManyChildren = 1;

    [SerializeField]
    bool m_ReRoll = false;

    // Start is called before the first frame update
    void Start()
    {
        TurnOnRandomChildren();
    }

    public void TurnAllChildrenOnOrOff(bool turn){
        foreach(Transform child in transform){
            child.gameObject.SetActive(turn);
        }
    }

    public void TurnOnRandomChildren(){
        int numberOfChildren = transform.childCount;
        if(numberOfChildren == 0){
            return;
        }
        if(numberOfChildren==1){
            transform.GetChild(0).gameObject.SetActive(true);
            return;
        }


        List<Transform> candidates = new List<Transform>(numberOfChildren);
        foreach(Transform child in transform){
            candidates.Add(child);
            child.gameObject.SetActive(false);
        }

        int howManyRandomPicks;
        if(m_turnOnBetweenThisManyChildren == m_andThisManyChildren)
            howManyRandomPicks = m_andThisManyChildren;
        else
            howManyRandomPicks = Random.Range(m_turnOnBetweenThisManyChildren+1, m_andThisManyChildren);

        for(int i =0; i<howManyRandomPicks; i++){
            int turnThisOneOn = Random.Range(0, candidates.Count);
            candidates[turnThisOneOn].gameObject.SetActive(true);
            candidates.RemoveAt(turnThisOneOn);
        }
    }

    // Update is called once per frame
    void Update()
    {
        #if UNITY_EDITOR
        if(m_ReRoll){
            m_ReRoll = false;
            TurnOnRandomChildren();
        }
        #endif
    }
}
