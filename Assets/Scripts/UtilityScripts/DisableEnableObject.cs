using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableEnableObject : MonoBehaviour
{
    [TextArea]
    public string info = "this will turn something off and then also back on in the first 2 frames. Use ToggleObjectOnOff if you just want to turn an object on or off.";
    [SerializeField]
    GameObject m_GO_To_DisableEnable;
    [SerializeField]
    bool m_startState = true;

    void Awake()
    {
        m_GO_To_DisableEnable.SetActive(m_startState);
    }

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(setGOActiveOffOn());
    }

    IEnumerator setGOActiveOffOn(){
        yield return new WaitForEndOfFrame();
        m_GO_To_DisableEnable.SetActive(!m_startState);
        yield return new WaitForEndOfFrame();
        m_GO_To_DisableEnable.SetActive(m_startState);
    }


}
