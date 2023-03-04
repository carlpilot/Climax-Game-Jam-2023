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
    public Cost[] costs;

}

[System.Serializable]
public struct Cost {
    public ResourceManager.ResourceType type;
    public int amount;
}