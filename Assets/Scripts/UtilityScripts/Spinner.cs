using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[ExecuteInEditMode]
public class Spinner : MonoBehaviour
{
    #if UNITY_EDITOR
    public bool executeInEditMode;
    #endif
    public float speed = 1;
    public Vector3 rotationDirection = Vector3.up;

    public bool spinWithUniqueOffset = true;

    // Start is called before the first frame update
    void Start()
    {
        if(spinWithUniqueOffset){
            transform.rotation *= Quaternion.AngleAxis(gameObject.GetInstanceID(), rotationDirection);
        }
    }

    #if UNITY_EDITOR

    void OnDrawGizmos () {

        if(!executeInEditMode || Application.isPlaying){
            return;
        }

        DoRotation();
    }

    #endif

    // Update is called once per frame
    void Update()
    {
        #if UNITY_EDITOR
            if(!executeInEditMode && !Application.isPlaying || !Application.isPlaying){
                return;
            }
        #endif
        
        DoRotation();
        
    }


    void DoRotation(){
        float angle = speed * Time.deltaTime;
        //Vector3 dir = rotationDirection = rotationDirection.normalized;
        // Vector3 currentRot = transform.rotation.eulerAngles;
        // currentRot.x += dir.x * angle;
        // currentRot.y += dir.y * angle;
        // currentRot.z += dir.z * angle;
        // transform.rotation = Quaternion.Euler(currentRot);

        transform.rotation *= Quaternion.AngleAxis(speed * Time.deltaTime, rotationDirection);
    }
}
