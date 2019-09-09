using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OculusInputManager : MonoBehaviourSingleton<OculusInputManager>
{
    [SerializeField]
    bool m_DebugLogs = false;
    [SerializeField]
    private OVRInput.Controller leftController;
    [SerializeField]
    private OVRInput.Controller rightController;
    [SerializeField] private Transform m_PlayerTransform;
    private float m_primaryIndexTriggerLeft;
    private float m_primaryIndexTriggerRight;
    private Vector3 m_leftControllerPosition;
    private Vector3 m_rightControllerPosition;
    private Vector2 m_leftThumbStick;
    private Vector2 m_rightThumbStick;

    [SerializeField]
    float m_floatToBoolThreshold = 0.02f;

    public OVRInput.Controller GetOculusHand(Valve.VR.SteamVR_Input_Sources handType)
    {
        if (handType == Valve.VR.SteamVR_Input_Sources.LeftHand)
        {
            return OVRInput.Controller.LTouch;
        }
        else if (handType == Valve.VR.SteamVR_Input_Sources.RightHand)
        {
            return OVRInput.Controller.RTouch;
        }
        else return OVRInput.Controller.None;
    }

    public bool GetGrabPinch(OVRInput.Controller hand)
    {
        bool val = OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger, hand);

        if (val)//val > m_floatToBoolThreshold)
        {
#if UNITY_EDITOR
            if (m_DebugLogs) Debug.Log("GetGrabPinch " + hand.ToString() + " " + val);
#endif
            return true;
        }
        else
            return false;
    }

    public bool GetGrabGrip(OVRInput.Controller hand)
    {
        bool val = OVRInput.Get(OVRInput.Button.PrimaryHandTrigger, hand);
        if (val)//val > m_floatToBoolThreshold)
        {
#if UNITY_EDITOR
            if (m_DebugLogs) Debug.Log("GetGrabGrip " + hand.ToString() + " " + val);
#endif
            return true;
        }
        else
            return false;
    }

    public bool GetGrabPinchUp(OVRInput.Controller hand)
    {
        bool val = OVRInput.GetUp(OVRInput.Button.PrimaryIndexTrigger, hand);

        if (val)//val > m_floatToBoolThreshold)
        {
#if UNITY_EDITOR
            if (m_DebugLogs) Debug.Log("GetGrabPinch " + hand.ToString() + " " + val);
#endif
            return true;
        }
        else
            return false;
    }

    public bool GetGrabGripUp(OVRInput.Controller hand)
    {
        bool val = OVRInput.GetUp(OVRInput.Button.PrimaryHandTrigger, hand);
        if (val)//val > m_floatToBoolThreshold)
        {
#if UNITY_EDITOR
            if (m_DebugLogs) Debug.Log("GetGrabGrip " + hand.ToString() + " " + val);
#endif
            return true;
        }
        else
            return false;
    }

    public bool GetGrabPinchDown(OVRInput.Controller hand)
    {
        bool val = OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, hand);

        if (val)//val > m_floatToBoolThreshold)
        {
#if UNITY_EDITOR
            if (m_DebugLogs) Debug.Log("GetGrabPinch " + hand.ToString() + " " + val);
#endif
            return true;
        }
        else
            return false;
    }

    public bool GetGrabGripDown(OVRInput.Controller hand)
    {
        bool val = OVRInput.GetDown(OVRInput.Button.PrimaryHandTrigger, hand);
        if (val)//val > m_floatToBoolThreshold)
        {
#if UNITY_EDITOR
            if (m_DebugLogs) Debug.Log("GetGrabGrip " + hand.ToString() + " " + val);
#endif
            return true;
        }
        else
            return false;
    }


    public float PrimaryIndexTriggerLeft {
        get {
#if UNITY_EDITOR
            if (Mathf.Abs(m_primaryIndexTriggerLeft) > 0.05f)
                if (m_DebugLogs) Debug.Log("m_primaryIndexTriggerLeft " + m_primaryIndexTriggerLeft);
#endif
            return m_primaryIndexTriggerLeft; }
    }
    public float PrimaryIndexTriggerRight
    {
        get
        {
#if UNITY_EDITOR
            if (Mathf.Abs(m_primaryIndexTriggerRight) > 0.05f)
                if (m_DebugLogs) Debug.Log("m_primaryIndexTriggerRight "+m_primaryIndexTriggerRight);
#endif
            return m_primaryIndexTriggerRight;
        }
    }
    public Vector3 LeftControllerPosition
    {
        get
        {
#if UNITY_EDITOR
            if ((m_leftControllerPosition + m_PlayerTransform.position).magnitude > 0.02f)
                if (m_DebugLogs) Debug.Log("m_leftControllerPosition + m_PlayerTransform.position " + m_leftControllerPosition + m_PlayerTransform.position);
#endif
            return m_leftControllerPosition + m_PlayerTransform.position;
        }
    }
    public Vector3 RightControllerPosition
    {
        get
        {
#if UNITY_EDITOR
            if ((m_rightControllerPosition + m_PlayerTransform.position).magnitude > 0.02f)
                if (m_DebugLogs) Debug.Log("m_rightControllerPosition + m_PlayerTransform.position " + m_rightControllerPosition + m_PlayerTransform.position);
#endif
            return m_rightControllerPosition + m_PlayerTransform.position;
        }
    }
    public OVRInput.Controller LeftController
    {
        get
        {
            return leftController;
        }
        set
        {
            leftController = value;
        }
    }
    public OVRInput.Controller RightController
    {
        get
        {
            return rightController;
        }
        set
        {
            rightController = value;
        }
    }
    public Vector2 LeftThumbStick
    {
        get
        {
#if UNITY_EDITOR
            if (m_leftThumbStick.magnitude>0.02f)
            if (m_DebugLogs) Debug.Log("m_leftThumbStick " + m_leftThumbStick);
#endif
            return m_leftThumbStick;
        }
    }
    public Vector2 RightThumbStick
    {
        get
        {
#if UNITY_EDITOR
            if (m_rightThumbStick.magnitude > 0.02f)
                if (m_DebugLogs) Debug.Log("m_rightThumbStick " + m_rightThumbStick);
#endif
            return m_rightThumbStick;
        }
    }

    bool m_left_TeleportActionDown;
    public bool LeftTeleportActionDown
    {
        get
        {
#if UNITY_EDITOR
            if (m_left_TeleportActionDown == true)
                if (m_DebugLogs) Debug.Log("m_left_TeleportActionDown " + m_left_TeleportActionDown);
#endif
            return m_left_TeleportActionDown;
        }
    }
    bool m_left_TeleportActionUp;
    public bool LeftTeleportActionUp
    {
        get
        {
#if UNITY_EDITOR
            if (m_left_TeleportActionUp == true)
            if (m_DebugLogs) Debug.Log("m_left_TeleportActionUp " + m_left_TeleportActionUp);
#endif
            return m_left_TeleportActionUp;
        }
    }

    bool m_right_TeleportActionDown;
    public bool RightTeleportActionDown
    {
        get
        {
#if UNITY_EDITOR
            if (m_right_TeleportActionDown == true)
                if (m_DebugLogs) Debug.Log("m_right_TeleportActionDown " + m_right_TeleportActionDown);
#endif
            return m_right_TeleportActionDown;
        }
    }
    bool m_right_TeleportActionUp;
    public bool RightTeleportActionUp
    {
        get
        {
#if UNITY_EDITOR
            if (m_right_TeleportActionUp == true)
                if (m_DebugLogs) Debug.Log("m_right_TeleportActionUp " + m_right_TeleportActionUp);
#endif
            return m_right_TeleportActionUp;
        }
    }

    bool m_right_TeleportAction;
    public bool RightTeleportAction
    {
        get
        {
#if UNITY_EDITOR
            if (m_DebugLogs) Debug.Log("m_right_TeleportAction " + m_right_TeleportAction);
#endif
            return m_right_TeleportAction;
        }
    }
    bool m_left_TeleportAction;
    public bool LeftTeleportAction
    {
        get
        {
#if UNITY_EDITOR
            if (m_DebugLogs) Debug.Log("m_left_TeleportAction " + m_left_TeleportAction);
#endif
            return m_left_TeleportAction;
        }
    }

    private void Update(){
        m_primaryIndexTriggerLeft = GetPrimaryIndexTrigger(LeftController);
        m_primaryIndexTriggerRight = GetPrimaryIndexTrigger(RightController);
        m_leftControllerPosition = GetControllerPosition(LeftController);
        m_rightControllerPosition = GetControllerPosition(RightController);
        m_leftThumbStick = GetControllerThumbStick(LeftController);
        m_rightThumbStick = GetControllerThumbStick(RightController);
        m_left_TeleportActionDown = GetControllerTeleportActionDown(LeftController);
        m_left_TeleportActionUp = GetControllerTeleportActionUp(LeftController);
        m_right_TeleportActionDown = GetControllerTeleportActionDown(RightController);
        m_right_TeleportActionUp = GetControllerTeleportActionUp(RightController);
        m_left_TeleportAction = GetControllerTeleportAction(LeftController);
        m_right_TeleportAction = GetControllerTeleportAction(RightController);
    }

    public float GetPrimaryIndexTrigger(OVRInput.Controller controller)
    {
        float value = OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger, controller);
        
        return value;
    }

    public Vector3 GetControllerPosition(OVRInput.Controller controller)
    {
        Vector3 value = OVRInput.GetLocalControllerPosition(controller);
        return value;
    }

    public Vector2 GetControllerThumbStick(OVRInput.Controller controller)
    {
        Vector2 value = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, controller);
        return value;
    }

    public bool GetControllerTeleportActionDown(OVRInput.Controller controller)
    {
        //Debug.Log(controller.ToString());
        //Debug.Log(OVRInput.Controller.LTouch.ToString());
        bool value = false;// OVRInput.GetDown(OVRInput.Button.One, controller) | OVRInput.Get(OVRInput.Button.Three, controller);
        if (controller == OVRInput.Controller.LTouch)
        {
            value = OVRInput.GetDown(OVRInput.Button.Two, controller) | OVRInput.GetDown(OVRInput.Button.One, controller);
            #if UNITY_EDITOR
                if (m_DebugLogs && value) Debug.Log("Teleport Action " + controller.ToString() + " " + value);
            #endif
            //Debug.Log(controller.ToString()+" "+ value);
            /*
            Debug.Log(controller.ToString() + " " + OVRInput.Button.PrimaryIndexTrigger.ToString() + " " + OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, controller));
            Debug.Log(controller.ToString() + " " + OVRInput.Button.PrimaryHandTrigger.ToString() + " " + OVRInput.GetDown(OVRInput.Button.PrimaryHandTrigger, controller));
            Debug.Log(controller.ToString() + " " + OVRInput.Button.One.ToString() + " " + OVRInput.GetDown(OVRInput.Button.One, controller));
            Debug.Log(controller.ToString() + " " + OVRInput.Button.Two.ToString() + " " + OVRInput.GetDown(OVRInput.Button.Two, controller));
            Debug.Log(controller.ToString() + " " + OVRInput.Button.Three.ToString() + " " + OVRInput.GetDown(OVRInput.Button.Three, controller));
            Debug.Log(controller.ToString() + " " + OVRInput.Button.Four.ToString() + " " + OVRInput.GetDown(OVRInput.Button.Four, controller));
            Debug.Log("_____");*/
        }
        else if (controller == OVRInput.Controller.RTouch) { 
            value = OVRInput.GetDown(OVRInput.Button.Two, controller) | OVRInput.GetDown(OVRInput.Button.One, controller);
            #if UNITY_EDITOR
                if (m_DebugLogs && value) Debug.Log("Teleport Action " + controller.ToString() + " " + value);
            #endif
            //Debug.Log(controller.ToString() + " " + value);
            /*
            Debug.Log(controller.ToString() + " " + OVRInput.Button.PrimaryIndexTrigger.ToString() + " " + OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, controller));
            Debug.Log(controller.ToString() + " " + OVRInput.Button.PrimaryHandTrigger.ToString() + " " + OVRInput.GetDown(OVRInput.Button.PrimaryHandTrigger, controller));
            Debug.Log(controller.ToString() + " " + OVRInput.Button.One.ToString() + " " + OVRInput.GetDown(OVRInput.Button.One, controller));
            Debug.Log(controller.ToString() + " " + OVRInput.Button.Two.ToString() + " " + OVRInput.GetDown(OVRInput.Button.Two, controller));
            Debug.Log(controller.ToString() + " " + OVRInput.Button.Three.ToString() + " " + OVRInput.GetDown(OVRInput.Button.Three, controller));
            Debug.Log(controller.ToString() + " " + OVRInput.Button.Four.ToString() + " " + OVRInput.GetDown(OVRInput.Button.Four, controller));
            Debug.Log("_____");*/
        }
       

        return value;
    }
    public bool GetControllerTeleportActionUp(OVRInput.Controller controller)
    {
        bool value = false;// OVRInput.GetUp(OVRInput.Button.One, controller) | OVRInput.Get(OVRInput.Button.Three, controller);
        if (controller == OVRInput.Controller.LTouch)
            value = OVRInput.GetUp(OVRInput.Button.Two, controller) | OVRInput.GetUp(OVRInput.Button.One, controller);
        if (controller == OVRInput.Controller.RTouch)
            value = OVRInput.GetUp(OVRInput.Button.Two, controller) | OVRInput.GetUp(OVRInput.Button.One, controller);
        
        return value;
    }

    public bool GetControllerTeleportAction(OVRInput.Controller controller)
    {
        bool value = false;
        if (controller == OVRInput.Controller.LTouch)
            value = OVRInput.Get(OVRInput.Button.Two, controller) | OVRInput.Get(OVRInput.Button.One, controller);
        if (controller == OVRInput.Controller.RTouch)
            value = OVRInput.Get(OVRInput.Button.Two, controller) | OVRInput.Get(OVRInput.Button.One, controller);
        
        return value;
    }
}