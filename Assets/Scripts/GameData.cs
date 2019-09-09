using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameData : MonoBehaviourSingleton<GameData> {




    public static class InputMapping
    {
        //public const Valve.VR.EVRButtonId _ResetPlayer_btn = Valve.VR.EVRButtonId.k_EButton_A;
        //public const Valve.VR.EVRButtonId _RescalePlayer_btn = Valve.VR.EVRButtonId.k_EButton_ApplicationMenu;
    }

    /*
    public enum RobotCommands{
        openClamp,
        closeClamp,
        topClamp,
        bottomClamp,
        releaseClamp
    }
    */

    public enum ConveryorBeltCommands{
        start,
        stop
    }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
