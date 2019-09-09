using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerInput : MonoBehaviour {

    [SerializeField] private Vector2 m_LThumpstick;
    [SerializeField] private Vector2 m_RThumpstick;

    public Vector3 GetLeftThumpStick{ get { return m_LThumpstick; }}
    public Vector3 GetRightThumpStick{ get { return m_RThumpstick; }}
    
    public delegate void ButtonPress();
    private ButtonPress onGripPressDown;

    private void Update() {
        m_LThumpstick = OVRInput.Get(OVRInput.RawAxis2D.LThumbstick);
        m_RThumpstick = OVRInput.Get(OVRInput.RawAxis2D.RThumbstick);

    }
}
 