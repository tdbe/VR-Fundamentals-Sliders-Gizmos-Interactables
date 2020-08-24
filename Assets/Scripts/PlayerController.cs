using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class PlayerController : MonoBehaviour {

    //[SerializeField]
    Vector3 m_playerSpawnPosition;

    [SerializeField]
    TeleportExtended valveTeleporter;

    // Use this for initialization
    void Start () {
        //m_playerSpawnPosition = InputManager.Instance.playArea.position;
        //valveTeleporter.TeleportToSpawnPoint(m_playerSpawnPosition);
        
        valveTeleporter.TeleportToSpawnPoint();
        //Debug.LogWarning("REIMPLEMENT: valveTeleporter.TeleportToSpawnPoint");
    }
	
	// Update is called once per frame
	void Update () {
        /*
        if ((InputManager.Instance.deviceLeft != null && InputManager.Instance.deviceLeft.GetPressUp(GameData.InputMapping._ResetPlayer_btn)) ||
            (InputManager.Instance.deviceRight != null && InputManager.Instance.deviceRight.GetPressUp(GameData.InputMapping._ResetPlayer_btn)) ||
            InputManager.Instance.GetKey_ResetPlayerPos()
            )
        {
            Debug.Log("Reset player position.");
            resetPlayerPosition();
        }
        */
    }


    public void resetPlayerPosition()
    {
        //InputManager.Instance.playArea.position = m_playerSpawnPosition;
        //valveTeleporter.TeleportToSpawnPoint();
        //valveTeleporter.TryBAMFPlayer(m_playerSpawnPosition, 0);
        valveTeleporter.TryBAMFPlayer(0);
        //Debug.LogWarning("REIMPLEMENT: valveTeleporter.TryBAMFPlayer(0);");
        
    }

}
