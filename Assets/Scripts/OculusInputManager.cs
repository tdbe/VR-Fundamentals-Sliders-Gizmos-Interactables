using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class OculusInputManager : MonoBehaviourSingleton<OculusInputManager>
{

    public enum AllowedGrabTypes
    {
        Pinch = 1 << 0,
        Grip = 1 << 1
    };
    [EnumFlags]
    public AllowedGrabTypes allowedGrabInputMask = AllowedGrabTypes.Pinch;



    [SerializeField]
    bool m_CalculateClickEvents;
    [SerializeField]
    float m_durationToRegisterClick = 0.6f;
    Dictionary<OVRInput.Controller,float> m_grabGripDownTime;
    Dictionary<OVRInput.Controller,bool>  m_grabGripClick;
    Dictionary<OVRInput.Controller,float> m_grabPinchDownTime;
    Dictionary<OVRInput.Controller,bool>  m_grabPinchClick;

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

    void Awake(){
        if(m_CalculateClickEvents){
            m_grabGripDownTime = new Dictionary<OVRInput.Controller, float>();
            //m_grabGripDownTime.Add(GetOculusHand(Valve.VR.SteamVR_Input_Sources.LeftHand))
            m_grabGripClick = new Dictionary<OVRInput.Controller, bool>();
            m_grabPinchDownTime = new Dictionary<OVRInput.Controller, float>();
            m_grabPinchClick = new Dictionary<OVRInput.Controller, bool>();
        }
    }

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

    public bool GetGrabAnyClicked(OVRInput.Controller hand){
        bool inputForGrab = ((allowedGrabInputMask & AllowedGrabTypes.Pinch) > 0) && OculusInputManager.Instance.GetGrabPinchClick(hand)
        ||
        ((allowedGrabInputMask & AllowedGrabTypes.Grip) > 0) && OculusInputManager.Instance.GetGrabGripClick(hand);
        return inputForGrab;
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
            if (m_DebugLogs) Debug.Log("GetGrabPinchUp " + hand.ToString() + " " + val);
#endif
            if(m_CalculateClickEvents){
                if(!m_grabPinchDownTime.ContainsKey(hand)){
                    m_grabPinchDownTime.Add(hand, 0);
                }
                if(Time.time - m_grabPinchDownTime[hand] <= m_durationToRegisterClick){
                    m_grabPinchDownTime[hand] = 0.0f;
                    if(!m_grabPinchClick.ContainsKey(hand)){
                        m_grabPinchClick.Add(hand, false);
                    }
                    m_grabPinchClick[hand] = true;
                    //Debug.Log("TRUE Time.time {"+Time.time+"} - m_grabPinchDownTime[hand{"+hand+"}] {"+m_grabPinchDownTime[hand]+"} {"+(Time.time - m_grabPinchDownTime[hand])+"} <= m_durationToRegisterClick {"+m_durationToRegisterClick+"}");
                }
            }
            return true;
        }
        else
            return false;
    }

    public bool GetGrabPinchClick(OVRInput.Controller hand){
        if(m_CalculateClickEvents ){
            if(!m_grabPinchClick.ContainsKey(hand))
                m_grabPinchClick.Add(hand, false);
            #if UNITY_EDITOR
                if (m_DebugLogs && m_grabPinchClick[hand]) Debug.Log("GetGrabPinchCLICK " + hand.ToString() + " " + m_grabPinchClick[hand]);
            #endif
            return m_grabPinchClick[hand];
        }
        else
        {
            return false;
        }
        
    }

    public bool GetGrabGripUp(OVRInput.Controller hand)
    {
        bool val = OVRInput.GetUp(OVRInput.Button.PrimaryHandTrigger, hand);
        if (val)//val > m_floatToBoolThreshold)
        {
#if UNITY_EDITOR
            if (m_DebugLogs) Debug.Log("GetGrabGripUp " + hand.ToString() + " " + val);
#endif
            if(m_CalculateClickEvents){
                if(!m_grabGripDownTime.ContainsKey(hand)){
                    m_grabGripDownTime.Add(hand, 0);
                }
                if(Time.time - m_grabGripDownTime[hand] <= m_durationToRegisterClick){
                    m_grabGripDownTime[hand] = 0.0f;
                    if(!m_grabGripClick.ContainsKey(hand)){
                        m_grabGripClick.Add(hand, false);
                    }
                    m_grabGripClick[hand] = true;
                }
            }
            return true;
        }
        else
            return false;
    }

    public bool GetGrabGripClick(OVRInput.Controller hand){
        if(m_CalculateClickEvents){
            if(!m_grabGripClick.ContainsKey(hand))
                m_grabGripClick.Add(hand, false);
            #if UNITY_EDITOR
                if (m_DebugLogs && m_grabGripClick[hand]) Debug.Log("GetGrabGripCLICK " + hand.ToString() + " " + m_grabGripClick[hand]);
            #endif
            
            return m_grabGripClick[hand];
        }
        else
        {
            return false;
        }
    }

    public bool GetGrabPinchDown(OVRInput.Controller hand)
    {
        bool val = OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, hand);

        if (val)//val > m_floatToBoolThreshold)
        {
#if UNITY_EDITOR
            if (m_DebugLogs) Debug.Log("GetGrabPinchDown " + hand.ToString() + " " + val);
#endif
            if(m_CalculateClickEvents){
                if(!m_grabPinchDownTime.ContainsKey(hand)){
                    m_grabPinchDownTime.Add(hand, 0);
                }
                m_grabPinchDownTime[hand] = Time.time;
            }
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
            if (m_DebugLogs) Debug.Log("GetGrabGripDown " + hand.ToString() + " " + val);
#endif
            if(m_CalculateClickEvents){
                if(!m_grabGripDownTime.ContainsKey(hand)){
                    m_grabGripDownTime.Add(hand, 0);
                }
                m_grabGripDownTime[hand] = Time.time;
            }
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

        if(m_CalculateClickEvents){
            foreach(OVRInput.Controller con in m_grabPinchClick.Keys.ToList()){
                m_grabPinchClick[con] = false;
                GetGrabPinchDown(con);
                GetGrabPinchUp(con);
                //GetGrabPinchClick(con);
            }
            foreach(OVRInput.Controller con in m_grabGripClick.Keys.ToList()){
                m_grabGripClick[con] = false;
                GetGrabGripDown(con);
                GetGrabGripUp(con);
                //GetGrabGripClick(con);
            }
        }
    }

    void LateUpdate(){
        /*
        if(m_CalculateClickEvents){
            foreach(OVRInput.Controller con in m_grabPinchClick.Keys.ToList()){
                m_grabPinchClick[con] = false;
            }
            foreach(OVRInput.Controller con in m_grabGripClick.Keys.ToList()){
                m_grabGripClick[con] = false;
            }
        }*/
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

/// <summary>
/// returns true if the primary thumbstick has been moved downwards more than halfway.
/// </summary>
/// <param name="controller"></param>
/// <returns></returns>
    public bool GetControllerThumbStickMovedDown(OVRInput.Controller controller)
    {
        bool value = OVRInput.Get(OVRInput.Button.PrimaryThumbstickDown, controller);
        return value;
    }

/// <summary>
/// returns true if the primary thumbstick has been moved upwards more than halfway.
/// </summary>
/// <param name="controller"></param>
/// <returns></returns>
    public bool GetControllerThumbStickMovedUp(OVRInput.Controller controller)
    {
        bool value = OVRInput.Get(OVRInput.Button.PrimaryThumbstickUp, controller);
        return value;
    }

    public bool GetControllerThumbStickPress(OVRInput.Controller controller)
    {
        bool value = OVRInput.Get(OVRInput.Button.PrimaryThumbstick, controller);
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

public class EnumFlags : PropertyAttribute
{
    public EnumFlags() { }
}


#if UNITY_EDITOR
	//-------------------------------------------------------------------------
[CustomPropertyDrawer( typeof( EnumFlags ) )]
public class EnumFlagsPropertyDrawer : PropertyDrawer
{
    public override void OnGUI( Rect position, SerializedProperty property, GUIContent label )
    {
        property.intValue = EditorGUI.MaskField( position, label, property.intValue, property.enumNames );
    }
}
#endif
