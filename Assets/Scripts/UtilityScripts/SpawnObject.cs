using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnObject : MonoBehaviour
{
    public GameObject gameObjectToSpawn;
    public Transform parent;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    public void Spawn()
    {
        GameObject go = Instantiate(gameObjectToSpawn);
        go.transform.parent = parent;
        go.transform.localPosition = Vector3.zero;
        go.transform.localRotation = Quaternion.identity;
        Debug.Log("~~~~~ Spawning: "+gameObjectToSpawn+"; under parent: "+parent);
    }
}
