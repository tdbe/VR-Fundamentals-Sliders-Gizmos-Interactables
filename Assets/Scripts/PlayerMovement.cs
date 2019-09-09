using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    [SerializeField] private Transform m_centerEyeAnchor;
    [SerializeField] private float m_moveSpeed = 0.1f;
    [SerializeField] private float m_snapTurnAngle = 45f;
    private OculusInputManager m_inputManager;
    private bool m_readyToTurn = true;

    private void Awake(){
        m_inputManager = FindObjectOfType<OculusInputManager>();
    }
    private void FixedUpdate()
    {
        WalkUpdate();
        TurnUpdate();
    }
    private void WalkUpdate(){
        Vector3 forwardMovementVector = m_inputManager.LeftThumbStick.y * m_centerEyeAnchor.transform.forward;
        forwardMovementVector += m_inputManager.LeftThumbStick.x * m_centerEyeAnchor.transform.right;
        forwardMovementVector.y = 0;
        transform.localPosition += forwardMovementVector.normalized * m_moveSpeed; 
    }

    private void TurnUpdate(){

        if(m_readyToTurn){
            // Do Turn
            if(m_inputManager.RightThumbStick.x > 0.9f){
                // Do Left Turn
                transform.eulerAngles = new Vector3(transform.eulerAngles.x,transform.eulerAngles.y + m_snapTurnAngle ,transform.eulerAngles.z);
                m_readyToTurn = false;
            }else if(m_inputManager.RightThumbStick.x < -0.9f){
                // Do Right Turn
                transform.eulerAngles = new Vector3(transform.eulerAngles.x,transform.eulerAngles.y - m_snapTurnAngle ,transform.eulerAngles.z);
                m_readyToTurn = false;
            }
        }else{  // is not ready to turn, player is has either done a turn o
            if(m_inputManager.RightThumbStick.x == 0){
                m_readyToTurn = true;
            }else{
                m_readyToTurn = false;
            }
        }
    }
}