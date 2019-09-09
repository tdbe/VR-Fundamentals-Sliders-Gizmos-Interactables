using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ChangeTransformValues : MonoBehaviour
{
    public Transform target;

    [Header("Populated on Enable (also in edit mode):")]
    [Space(20)]
    [SerializeField]
    Vector3 originalRelativePosition;
    [SerializeField]
    Vector3 originalRelativeRotation;
    [SerializeField]
    Vector3 originalLocalScale;


    
    [Header("New Values:")]
    [Space(20)]
    [SerializeField]
    Vector3 newRelativePosition;
    [SerializeField]
    Vector3 newRelativeRotation;
    [SerializeField]
    Vector3 newLocalScale;

    void Start(){
        if(target==null)
            target = transform;
    }

    // Start is called before the first frame update
    void OnEnable()
    {
         if(target==null)
            target = transform;
        PopulateValues();
    }

    void PopulateValues(){
        originalRelativePosition = target.localPosition;
        originalRelativeRotation = target.localRotation.eulerAngles;
        originalLocalScale = target.localScale;
    }

    public void SwitchToNewValues(){
        target.localPosition = newRelativePosition;
        target.localRotation = Quaternion.Euler(newRelativeRotation);
        target.localScale = newLocalScale;
    }

    
    public void SwitchToOriginalValues(){
        target.localPosition = originalRelativePosition;
        target.localRotation = Quaternion.Euler(originalRelativeRotation);
        target.localScale = originalLocalScale;

    }


}
