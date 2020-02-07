using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// This is where you get all your input data and references from
/// </summary>
public class InputManager : MonoBehaviourSingleton<InputManager> {

    [SerializeField]
    Transform _playArea;
    public Transform playArea { get { return _playArea; } }

    [SerializeField]
    Transform _headEyesTransform;
    public Transform headEyesTransform { get { return _headEyesTransform; } }

    //[SerializeField] Transform m_GearVRCamera;
    //public Transform GearVRCamera { get { return m_GearVRCamera; } }
    
    [SerializeField]
    Transform _origLeftOpenVRControllerTransform;
    [HideInInspector]
    public Transform leftHandOpenVRTransform;


    [SerializeField]
    Transform _origRightOpenVRControllerTransform;
    [HideInInspector]
    public Transform rightHandOpenVRTransform;
    
    //SteamVR_TrackedObject m_trackedControllerLeft; // Used instead of or in parallel to: private SteamVR_TrackedObject trackedController;
    //SteamVR_TrackedObject m_trackedControllerRight; // Used instead of or in parallel to: private SteamVR_TrackedObject trackedController;

    SteamVR_Controller.Device _deviceLeft; // Used instead of: private SteamVR_Controller.Device deviceLeft;
    public SteamVR_Controller.Device deviceLeft { get { return _deviceLeft; } }
    SteamVR_Controller.Device _deviceRight; // Used instead of: private SteamVR_Controller.Device deviceLeft;
    public SteamVR_Controller.Device deviceRight { get { return _deviceRight; } }

    SteamVR_Controller.Device[] _deviceList;
    Transform[] _deviceTransformList;


    [SerializeField]
    Transform m_playerGeometryLeftArm;
    [SerializeField]
    Transform m_playerGeometryRightArm;

    //[SerializeField]
    //Buttons startStopRobotsButton;


    bool _setHeight;
    public bool setHeight {get{return _setHeight;}}

    bool _masterSwitchOnButtonDown;
    public bool masterSwitchOnButtonDown
    {
        get
        {
            return _masterSwitchOnButtonDown;//startStopRobotsButton.GetOnButtonDown();
        }
    }

    //bool m_masterSwitchLastVal;
    public void ToggleMasterSwitch(bool val){
        /* 
        if(val != m_masterSwitchLastVal && val){
            _masterSwitchOnButtonDown = !_masterSwitchOnButtonDown;
            
        }
        m_masterSwitchLastVal = val;
        */
        
        _masterSwitchOnButtonDown = val;
        //Debug.Log("masterSwitchOnButtonDown: "+masterSwitchOnButtonDown);
    }

    /// <summary>
    /// an alternative to calling deviceLeft and deviceRight individually
    /// </summary>
    public SteamVR_Controller.Device[] deviceList { get { return _deviceList; } }
    public Transform[] deviceTransformList { get { return _deviceTransformList; } }

    public enum Hand
    {
        Left = 0,
        Right = 1
    }

    //int index_left;
    //int index_right;

    public static class Controls
    {
        public const Valve.VR.EVRButtonId _PointerActivation = Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad;
    }


    void Start()
    {
        _deviceList = new SteamVR_Controller.Device[2];
        _deviceTransformList = new Transform[2];
        _deviceTransformList[0] = _origLeftOpenVRControllerTransform;
        _deviceTransformList[1] = _origRightOpenVRControllerTransform;
        
        //m_trackedControllerLeft = _origLeftOpenVRControllerTransform.GetComponent<SteamVR_TrackedObject>();//trackedController = GetComponent<SteamVR_TrackedObject>();
        ////index_left = trackedControllerLeft.index; // this index works in PlayFromRecording mode even if VR is turned off and the index would be -1
        //Debug.Log("[0] trackedControllerLeft.index: " + m_trackedControllerLeft.index);

        //m_trackedControllerRight = _origRightOpenVRControllerTransform.GetComponent<SteamVR_TrackedObject>();//trackedController = GetComponent<SteamVR_TrackedObject>();
        ////index_right = trackedControllerRight.index; // this index works in PlayFromRecording mode even if VR is turned off and the index would be -1
        //Debug.Log("[1] trackedControllerRight.index: "+ m_trackedControllerRight.index);

        leftHandOpenVRTransform = _origLeftOpenVRControllerTransform;
        rightHandOpenVRTransform = _origRightOpenVRControllerTransform;
    }

    void getKeyboardButtons(){
        //_setHeight = Input.GetKeyDown(KeyCode.Space) ||  (deviceLeft != null && InputManager.Instance.deviceLeft.GetPressUp(GameData.InputMapping._RescalePlayer_btn)) ||
        //    (deviceRight != null && InputManager.Instance.deviceRight.GetPressUp(GameData.InputMapping._RescalePlayer_btn));
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();
        
        getKeyboardButtons();


        updateTrackedControllers();

            // Usage::
            //openvrDeviceLeft.GetAxis(Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger).x;// only X axis is used for trigger
            //Quaternion leftControllerRotation = openvrDeviceLeft.transform.rot;
    }


    public bool GetKey_ResetPlayerPos(){
        return Input.GetKeyDown(KeyCode.P);
    }
    
    /// <summary>
    /// Makes sure controllers are available (and recordable). For ex even if a controller was asleep when you started the game.
    /// The GetDevice functions don't waste performance if the controllers are already available.
    /// Here you can also choose to always have the leftmost controller as the Left controller, or keep the original left controller as the left controller.
    /// </summary>
    void updateTrackedControllers()
    {
        /*
        //int lControllerIndex = index_left;// KHVR.SteamControllerWrapperSerializable.GetDeviceIndex(SteamVR_Controller.DeviceRelation.Leftmost); // Used instead of  SteamVR_Controller.GetDeviceIndex(SteamVR_Controller.DeviceRelation.Leftmost);
        //int lControllerIndex = KHVR.Input.SteamControllerWrapperSerializable.GetDeviceIndex(SteamVR_Controller.DeviceRelation.Leftmost);
        int lControllerIndex = (int)(SteamVR_Controller.DeviceRelation.Leftmost);

        //int rControllerIndex = index_right;// KHVR.SteamControllerWrapperSerializable.GetDeviceIndex(SteamVR_Controller.DeviceRelation.Leftmost); // Used instead of  SteamVR_Controller.GetDeviceIndex(SteamVR_Controller.DeviceRelation.Leftmost);
        //int rControllerIndex = KHVR.Input.SteamControllerWrapperSerializable.GetDeviceIndex(SteamVR_Controller.DeviceRelation.Rightmost);
        int rControllerIndex = (int)(SteamVR_Controller.DeviceRelation.Rightmost);


        if ((SteamVR_TrackedObject.EIndex)lControllerIndex == m_trackedControllerLeft.index)
        {
            leftHandOpenVRTransform = _origLeftOpenVRControllerTransform;

            if(m_trackedControllerLeft.index >= 0 && m_trackedControllerRight.index >= 0)
            {
                if (
              m_trackedControllerLeft.index < m_trackedControllerRight.index
               )
                {
                    _deviceLeft = KHVR.Input.SteamControllerWrapperSerializable.GetWrappedDevice(SteamVR_Controller.Input(lControllerIndex));
                    _deviceRight = KHVR.Input.SteamControllerWrapperSerializable.GetWrappedDevice(SteamVR_Controller.Input(rControllerIndex));
                }
                else
                {
                    _deviceLeft = KHVR.Input.SteamControllerWrapperSerializable.GetWrappedDevice(SteamVR_Controller.Input(rControllerIndex));
                    _deviceRight = KHVR.Input.SteamControllerWrapperSerializable.GetWrappedDevice(SteamVR_Controller.Input(lControllerIndex));
                }

            }


        }
        else if (lControllerIndex == m_trackedControllerRight.index)
        {
            leftHandOpenVRTransform = _origRightOpenVRControllerTransform;
            //_deviceLeft = KHVR.Input.SteamControllerWrapperSerializable.GetWrappedDevice(SteamVR_Controller.Input(rControllerIndex));
            _deviceLeft = KHVR.Input.SteamControllerWrapperSerializable.GetWrappedDevice(SteamVR_Controller.Input(lControllerIndex));
            _deviceRight = KHVR.Input.SteamControllerWrapperSerializable.GetWrappedDevice(SteamVR_Controller.Input(rControllerIndex));

        }
        else if (
            (
                m_trackedControllerLeft.index >= 0 && m_trackedControllerRight.index >= 0 
                && m_trackedControllerLeft.index < m_trackedControllerRight.index
            ) || 
                (m_trackedControllerLeft.index >= 0 && m_trackedControllerRight.index <0)
            )
            
        {
            leftHandOpenVRTransform = _origLeftOpenVRControllerTransform;
            _deviceLeft = KHVR.Input.SteamControllerWrapperSerializable.GetWrappedDevice(SteamVR_Controller.Input(lControllerIndex));

        }
        else if (
            (
                m_trackedControllerLeft.index >= 0 && m_trackedControllerRight.index >= 0
                && m_trackedControllerLeft.index > m_trackedControllerRight.index
            )
            )

        {
            leftHandOpenVRTransform = _origRightOpenVRControllerTransform;
            _deviceLeft = KHVR.Input.SteamControllerWrapperSerializable.GetWrappedDevice(SteamVR_Controller.Input(rControllerIndex));

        }
        else
        {
            _deviceLeft = null;
            leftHandOpenVRTransform = null; // TODO: maybe this is a bad idea?
        }



        if (rControllerIndex == m_trackedControllerRight.index)
        {
            rightHandOpenVRTransform = _origRightOpenVRControllerTransform;
            if (m_trackedControllerLeft.index >= 0 && m_trackedControllerRight.index >= 0)
            {
                if (
                    m_trackedControllerLeft.index < m_trackedControllerRight.index
                    )
                {
                    _deviceRight = KHVR.Input.SteamControllerWrapperSerializable.GetWrappedDevice(SteamVR_Controller.Input(rControllerIndex));
                    _deviceLeft = KHVR.Input.SteamControllerWrapperSerializable.GetWrappedDevice(SteamVR_Controller.Input(lControllerIndex));
                }
                else
                {
                    _deviceRight = KHVR.Input.SteamControllerWrapperSerializable.GetWrappedDevice(SteamVR_Controller.Input(lControllerIndex));
                    _deviceLeft = KHVR.Input.SteamControllerWrapperSerializable.GetWrappedDevice(SteamVR_Controller.Input(rControllerIndex));
                }
            }
           
        }
        else if (rControllerIndex == m_trackedControllerLeft.index)
        {
            rightHandOpenVRTransform = _origLeftOpenVRControllerTransform;
            //_deviceRight = KHVR.Input.SteamControllerWrapperSerializable.GetWrappedDevice(SteamVR_Controller.Input(lControllerIndex));
            _deviceRight = KHVR.Input.SteamControllerWrapperSerializable.GetWrappedDevice(SteamVR_Controller.Input(rControllerIndex));
            _deviceLeft = KHVR.Input.SteamControllerWrapperSerializable.GetWrappedDevice(SteamVR_Controller.Input(lControllerIndex));
        }
        else if (
            (
                m_trackedControllerLeft.index >= 0 && m_trackedControllerRight.index >= 0
                && m_trackedControllerLeft.index < m_trackedControllerRight.index
            ) ||
                (m_trackedControllerRight.index >= 0 && m_trackedControllerLeft.index < 0)
            )

        {
            rightHandOpenVRTransform = _origRightOpenVRControllerTransform;
            _deviceRight = KHVR.Input.SteamControllerWrapperSerializable.GetWrappedDevice(SteamVR_Controller.Input(rControllerIndex));
        }
        else if (
            (
                m_trackedControllerLeft.index >= 0 && m_trackedControllerRight.index >= 0
                && m_trackedControllerLeft.index > m_trackedControllerRight.index
            ) 
            )

        {
            rightHandOpenVRTransform = _origLeftOpenVRControllerTransform;
            _deviceRight = KHVR.Input.SteamControllerWrapperSerializable.GetWrappedDevice(SteamVR_Controller.Input(lControllerIndex));
        }
        else
        {
            _deviceRight = null;
            rightHandOpenVRTransform = null; // TODO: maybe this is a bad idea?
        }



       
        //if this device is not tracked right now - it has no device index - skip it.
        if ((_deviceLeft != null && _deviceLeft.hasTracking == false) || 
            lControllerIndex < 0)// || lControllerIndex >= Valve.VR.OpenVR.k_unMaxTrackedDeviceCount)
        {
            _deviceLeft = null;
            if (leftHandOpenVRTransform != null)
            {
                leftHandOpenVRTransform.GetChild(0).gameObject.SetActive(false);
                m_playerGeometryLeftArm.gameObject.SetActive(false);
                
                leftHandOpenVRTransform = null;
            }

        }
        else if(leftHandOpenVRTransform)
        {
            leftHandOpenVRTransform.GetChild(0).gameObject.SetActive(true);
            m_playerGeometryLeftArm.gameObject.SetActive(true);

        }

        //if this device is not tracked right now - it has no device index - skip it.
        if ((_deviceRight != null && _deviceRight.hasTracking == false) || 
            rControllerIndex < 0)// || rControllerIndex >= Valve.VR.OpenVR.k_unMaxTrackedDeviceCount)
        {
            _deviceRight = null;
            if (rightHandOpenVRTransform != null)
            {
                rightHandOpenVRTransform.GetChild(0).gameObject.SetActive(false);
                m_playerGeometryRightArm.gameObject.SetActive(false);
                
                rightHandOpenVRTransform = null;
            }
        }
        else if(rightHandOpenVRTransform)
        {
            rightHandOpenVRTransform.GetChild(0).gameObject.SetActive(true);
            m_playerGeometryRightArm.gameObject.SetActive(true);

        }

        _deviceList[0] = _deviceLeft;
        _deviceList[1] = _deviceRight;
        _deviceTransformList[0] = leftHandOpenVRTransform;
        _deviceTransformList[1] = rightHandOpenVRTransform;
        */
        
                    //int index = SteamVR_Controller.GetDeviceIndex(SteamVR_Controller.DeviceRelation.Leftmost);
                    //if(index >=0)
                    //    _deviceList[0] = _deviceLeft = SteamVR_Controller.Input(index);

        //Debug.Log(index + " (left); is null?: " + _deviceLeft + "; " +
        //InputManager.Instance.deviceLeft.GetHairTrigger());

                    //index = SteamVR_Controller.GetDeviceIndex(SteamVR_Controller.DeviceRelation.Rightmost);
                    //if (index >= 0)
                    //    _deviceList[1] = _deviceRight = SteamVR_Controller.Input(index);

        //Debug.Log(index + " (right); is null?: " + _deviceRight + "; " +
        //SteamVR_Controller.Input(index).GetPress(Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger));

        /*
        for ( int i = 0; i< 8; i++)
        {
            SteamVR_Controller.Device dev = SteamVR_Controller.Input(i);
            if(dev != null)
            {
                Debug.Log(i + " (); " + "; " + SteamVR_Controller.Input(i).GetHairTrigger());

            }
        }
        */
    }
    
}
