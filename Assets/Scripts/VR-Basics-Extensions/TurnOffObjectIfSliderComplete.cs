using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnOffObjectIfSliderComplete : MonoBehaviour
{
    public RailGroup railGroup;
    public GameObject[] objectsToTurnOnOrOff;
    

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TurnOnOffObjectIfSliderComplete(bool turnOn){
        if(railGroup.sliderPosition == 1){
            foreach(GameObject go in objectsToTurnOnOrOff)
                go.SetActive(turnOn);
        }
    }
}
