using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu ()]
public class Buildable : ScriptableObject {

    [Header ("Object")]
    public new string name;
    public Sprite icon;
    public GameObject prefab;

    [Header ("Resources")]
    public int woodRequired;
    public int metalRequired;

}