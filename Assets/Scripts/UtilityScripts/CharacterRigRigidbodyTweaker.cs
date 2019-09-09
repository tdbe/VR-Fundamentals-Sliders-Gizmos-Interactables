using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[ExecuteInEditMode]
public class CharacterRigRigidbodyTweaker : MonoBehaviour
{

    public LayerMask allowedLayerMask = ~0;
    public List<string> disallowedTags;
    Dictionary<int, string> m_ignoreTagsDict;
    [System.Serializable]
    public struct TrackedRigidbody{
        public string name;
        public Rigidbody rigidbody;
        public float originalMass;
        public float originalDrag;
        public TrackedRigidbody(Rigidbody rb){
            name = rb.name;
            rigidbody = rb;
            originalMass = rb.mass;
            originalDrag = rb.drag;
        }
    }
    public List<TrackedRigidbody> trackedRigidbodies;
    public bool applyChangesAtRuntime = true;
    public float massScale = 1;
    public float dragAdd = 1;
    #if UNITY_EDITOR
    [Header("It's annoying to refert this so be careful with where you want to do this (e.g. in the prefab or outside the prefab)")]
    public bool reloadRigidbodies;
    public bool applyChangesInEditor;
    #endif

    // Start is called before the first frame update
    void OnEnable()
    {   
        #if UNITY_EDITOR
        if(Application.isPlaying && applyChangesAtRuntime){
            FindOrRefreshRelevantRigidbodies();
            ApplyChangesToTrackedRigidbodies();
        }
        #else
        if(applyChangesAtRuntime){
            FindOrRefreshRelevantRigidbodies();
            ApplyChangesToTrackedRigidbodies();
        }
        #endif
    }

    public void FindOrRefreshRelevantRigidbodies(){
        m_ignoreTagsDict = new Dictionary<int, string>();
        foreach(string str in disallowedTags){
            m_ignoreTagsDict.Add(str.GetHashCode(),str);
        }
        //trackedRigidbodies = GetComponentsInChildren<Rigidbody>().OfType<Rigidbody>().ToList();//this linq stuff isn't fast, so don't do it every frame.
        trackedRigidbodies = new List<TrackedRigidbody>();
        Rigidbody[] rbs = GetComponentsInChildren<Rigidbody>();
        foreach(Rigidbody rb in rbs){
            if( allowedLayerMask == ((1 << rb.gameObject.layer) | allowedLayerMask) &&
                !m_ignoreTagsDict.ContainsKey(rb.gameObject.tag.GetHashCode())
            ){
                trackedRigidbodies.Add(new TrackedRigidbody(rb));
            }
        }
    }

    public void ApplyChangesToTrackedRigidbodies(){
        foreach(TrackedRigidbody rb in trackedRigidbodies){
            rb.rigidbody.mass = rb.originalMass * massScale;
            rb.rigidbody.drag = rb.originalDrag + dragAdd;
        }
    }

    // Update is called once per frame
    void Update()
    {   
        #if UNITY_EDITOR
        if(reloadRigidbodies){
            reloadRigidbodies = false;
            FindOrRefreshRelevantRigidbodies();
            //ApplyChangesToTrackedRigidbodies();
        }
        if(applyChangesInEditor){
            applyChangesInEditor = false;
            //FindOrRefreshRelevantRigidbodies();
            ApplyChangesToTrackedRigidbodies();
        }
        #endif
        
    }
}
