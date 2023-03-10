using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildMenu : MonoBehaviour {

    Construction con;
    public Transform iconParent;
    public GameObject iconPrefab;
    public GameObject resourcePrefab;

    [HideInInspector]
    public ResourceManager rm;

    private void Awake () {
        con = FindObjectOfType<Construction> ();
        rm = FindObjectOfType<ResourceManager> ();
        Populate ();
    }

    void Populate () {
        for (int i = 0; i < con.buildables.Length; i++) {
            Instantiate (iconPrefab, iconParent).GetComponent<BuildIcon> ().Assign (con.buildables[i]);
        }
    }
}
