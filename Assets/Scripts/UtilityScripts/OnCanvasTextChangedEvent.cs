using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class OnCanvasTextChangedEvent : MonoBehaviour
{

    Text m_text;
    int m_lastTextLength;//not this should be a hash of the letters in case you have 2 messages in a row with the same number of letters... But doiung that every update seems excessive. Better to just know when an external script updates this text field instead.
    
    public UnityEvent OnTextUpdateEvent;


    
    // Start is called before the first frame update
    void Start()
    {
        m_text = GetComponent<Text>();
        m_lastTextLength = m_text.text.Length;
    }

    // Update is called once per frame
    void Update()
    {
        if(m_text.text.Length != m_lastTextLength){
            if(OnTextUpdateEvent!=null)
                OnTextUpdateEvent.Invoke();
        }
        m_lastTextLength = m_text.text.Length;
    }
}
