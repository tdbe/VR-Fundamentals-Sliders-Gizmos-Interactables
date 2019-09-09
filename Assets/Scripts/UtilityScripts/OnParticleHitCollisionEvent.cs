using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class OnParticleHitCollisionEvent : MonoBehaviour {
    public LayerMask collisionLayerMask = ~0;
    [Tooltip ("Leave the array null if you want to broadcast event normally.")]
    [Header ("Leave the array null if you want to broadcast event normally.")]
    public GameObject[] onlyTriggerOnTheseObjects;
    public UnityEvent doOnCollision;
    public bool m_AllowCollisionCheck = true;
#if UNITY_EDITOR
    public bool debug = false;
#endif

    // Start is called before the first frame update
    void Start () {

    }

    bool IsRBAllowed (GameObject other) {
        if (onlyTriggerOnTheseObjects == null || onlyTriggerOnTheseObjects.Length == 0)
            return true;

        foreach (GameObject go in onlyTriggerOnTheseObjects) {
            if (other == go)
                return true;
        }
        return false;
    }

    void OnParticleCollision (GameObject other) {
        if (!m_AllowCollisionCheck)
            return;

        if (collisionLayerMask != ((1 << other.gameObject.layer) | collisionLayerMask))
            return;

        if (!IsRBAllowed (other))
            return;

        if (doOnCollision != null)
            doOnCollision.Invoke ();

#if UNITY_EDITOR
        if (debug)
            Debug.Log ("[" + gameObject.name + "] Collided with: " + other.gameObject.name);
#endif
    }
}