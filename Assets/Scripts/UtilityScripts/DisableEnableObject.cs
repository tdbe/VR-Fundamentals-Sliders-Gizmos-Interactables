using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableEnableObject : MonoBehaviour
{

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
        m_GO_To_DisableEnable.SetActive(false);
        yield return new WaitForEndOfFrame();
        m_GO_To_DisableEnable.SetActive(true);
    }


}
