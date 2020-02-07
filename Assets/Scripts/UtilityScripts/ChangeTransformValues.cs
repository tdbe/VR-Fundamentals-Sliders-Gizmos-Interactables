using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[ExecuteInEditMode]
public class ChangeTransformValues : MonoBehaviour
{
    public Transform target;

    //[Header("Populated on Enable (also in edit mode):")]
    [Space(20)]
    [SerializeField]
    Vector3 originalRelativePosition;
    [SerializeField]
    Vector3 originalRelativeRotation;
    [SerializeField]
    Vector3 originalLocalScale=Vector3.one;


    
    [Header("New Values:")]
    [Space(20)]
    [SerializeField]
    Vector3 newRelativePosition;
    [SerializeField]
    Vector3 newRelativeRotation;
    [SerializeField]
    Vector3 newLocalScale;

    [Space(30)]
    [SerializeField]
    Vector3 m_worldspaceDirectionPositionOffset;

    [Space(30)]
    [SerializeField]
    bool m_updatePosition = true;
    [SerializeField]
    bool m_updateRotation;
    [SerializeField]
    bool m_updateScale;

    [SerializeField]
    bool m_switchToNewValuesOnlyOnStart = false;

    [SerializeField]
    bool m_useWorldSpace = false;

    void Start(){
        if(target==null)
            target = transform;

        if(m_switchToNewValuesOnlyOnStart){
            PopulateValues();
            SwitchToNewValues();
        }
    }

    // Start is called before the first frame update
    void OnEnable()
    {
        if(m_switchToNewValuesOnlyOnStart)
            return;
         if(target==null)
            target = transform;
        PopulateValues();
    }

    void PopulateValues(){
        if(m_useWorldSpace){
            originalRelativePosition = target.position;
            originalRelativeRotation = target.rotation.eulerAngles;
            originalLocalScale = target.localScale;
        }
        else{
            originalRelativePosition = target.localPosition;
            originalRelativeRotation = target.localRotation.eulerAngles;
            originalLocalScale = target.localScale;
        }
    }

    public void SwitchToNewValues(){
        if(m_useWorldSpace){
            if(m_updatePosition) target.position = newRelativePosition;
            if(m_updateRotation) target.rotation = Quaternion.Euler(newRelativeRotation);
            if(m_updateScale) target.localScale = newLocalScale;
        }else{
            if(m_updatePosition) target.localPosition = newRelativePosition;
            if(m_updateRotation) target.localRotation = Quaternion.Euler(newRelativeRotation);
            if(m_updateScale) target.localScale = newLocalScale;
        }

        if(m_updatePosition) target.position += m_worldspaceDirectionPositionOffset;
    }

    
    public void SwitchToOriginalValues(){
        if(m_useWorldSpace){
            if(m_updatePosition) target.position = originalRelativePosition;
            if(m_updateRotation) target.rotation = Quaternion.Euler(originalRelativeRotation);
            if(m_updateScale) target.localScale = originalLocalScale;
        }else{
            if(m_updatePosition) target.localPosition = originalRelativePosition;
            if(m_updateRotation) target.localRotation = Quaternion.Euler(originalRelativeRotation);
            if(m_updateScale) target.localScale = originalLocalScale;
        }

    }


}
