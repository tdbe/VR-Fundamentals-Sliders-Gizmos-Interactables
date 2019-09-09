using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Spinner : MonoBehaviour
{
    #if UNITY_EDITOR
    public bool executeInEditMode;
    #endif
    public float speed = 1;
    public Vector3 rotationDirection = Vector3.up;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        #if UNITY_EDITOR
            if(!executeInEditMode){
                return;
            }
        #endif
        Vector3 currentRot = transform.rotation.eulerAngles;
        currentRot.x += rotationDirection.x * speed * Time.deltaTime;
        currentRot.y += rotationDirection.y * speed * Time.deltaTime;
        currentRot.z += rotationDirection.z * speed * Time.deltaTime;
        transform.rotation = Quaternion.Euler(currentRot);
    }
}
