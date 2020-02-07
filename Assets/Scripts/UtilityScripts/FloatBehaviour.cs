using UnityEngine;
using System;
using System.Collections;

[ExecuteInEditMode]
public class FloatBehaviour : MonoBehaviour
{
    public bool executeInEditMode;
    public Vector3 dir = Vector3.up;
    Vector3 originalPos;
    public bool uniqueOffset = true;

    public float floatStrength = 1; // You can change this in the Unity Editor to 
                                    // change the range of y positions that are possible.

    void OnEnable()
    {
        #if UNITY_EDITOR
        if(!Application.isPlaying&&!executeInEditMode)
            return;
        #endif
        
        this.originalPos = this.transform.position;

    }

    void Update()
    {
        #if UNITY_EDITOR
        if(!Application.isPlaying&&!executeInEditMode)
            return;
        #endif

        float time = Time.time;
        if(uniqueOffset)
            time += gameObject.GetInstanceID();
        Vector3 speed = dir * ((float)Math.Sin(time) * floatStrength);
        transform.position = originalPos + speed;
    }
}