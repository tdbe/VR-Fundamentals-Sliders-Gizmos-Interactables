using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
 public class CameraFPS : MonoBehaviour { 
     public float fps = 20;
     float lastRenderTime;
     Camera cam;
 
     void Start () {
         cam = GetComponent<Camera>();
         cam.enabled = false;
         lastRenderTime = Time.time;
     }
     
     void Update () {

         if(Time.time - lastRenderTime >= 1 / fps){
            lastRenderTime = Time.time;
            cam.Render();
         }
 
     }
 }