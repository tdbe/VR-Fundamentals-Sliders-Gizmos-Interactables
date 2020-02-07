 using UnityEngine;
 using System.Collections;
 using System.Collections.Generic;
 
 public class LightingChange : MonoBehaviour
 {
     [Header("This is a list of parents that may contain Light components you don't want rendered with the camera that this script is attached to.")]
     public List<Transform> LightsToNotRenderWithThisCamera;
     List<Light> Lights;
     
     void OnEnable(){
        if(LightsToNotRenderWithThisCamera==null && LightsToNotRenderWithThisCamera.Count<1)
            return;
        Lights = new List<Light>();
        foreach (var item in LightsToNotRenderWithThisCamera)
        {
            if(item==null)
                continue;
            Light[] lights = item.GetComponentsInChildren<Light>();
            foreach(Light li in lights)
                Lights.Add(li);
        }
     }
 
     void OnPreCull()
     {
         if(Lights==null)
            return;
         foreach (Light light in Lights)
         {
             //Debug.Log(light.name);
             light.enabled = false;
         }
     }
 
     void OnPostRender()
     {
         if(Lights==null)
            return;
         foreach (Light light in Lights)
         {
             light.enabled = true;
         }
     }
 }