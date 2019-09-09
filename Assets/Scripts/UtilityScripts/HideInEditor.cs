using UnityEngine;
using System.Collections;
[ExecuteInEditMode]
public class HideInEditor : MonoBehaviour
{
#if UNITY_EDITOR
    void OnEnable()
    {
        GetComponent<Renderer>().enabled = !Application.isEditor || Application.isPlaying;
    }
    void OnDisable()
    {
        GetComponent<Renderer>().enabled = true;
    }
#endif
}