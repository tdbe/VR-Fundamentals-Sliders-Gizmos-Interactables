using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeMaterialProps : MonoBehaviour
{
    public Material m_materialToChange;
    public Color m_originalColor;
    public Color m_newColor;
    public bool m_ChangeToNewColorOnStart;

    // Start is called before the first frame update
    void Start()
    {
        m_originalColor = m_materialToChange.color;

        if(m_ChangeToNewColorOnStart)
            ChangeToNewColor();
    }

    private void OnDestroy() {
        if(m_ChangeToNewColorOnStart)
            ChangeToOriginalColor();
    }

    public void ChangeToNewColor(){
        m_materialToChange.color = m_newColor;
    }

    public void ChangeToOriginalColor(){
        m_materialToChange.color = m_originalColor;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
