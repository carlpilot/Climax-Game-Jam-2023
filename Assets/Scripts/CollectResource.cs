using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectResource : MonoBehaviour {

    ResourceManager rm;

    public int value;
    public ResourceManager.ResourceType type;


    private void Awake () {
        rm = FindObjectOfType<ResourceManager> ();
        transform.Rotate(0, Random.Range(0, 360), 0, Space.World);
    }

    private void OnTriggerEnter (Collider other) {
        if (other.gameObject.layer == 3) Collected ();
    }

    void Collected () {
        rm.Add (value, type);
        Destroy (this.gameObject);
    }
}
