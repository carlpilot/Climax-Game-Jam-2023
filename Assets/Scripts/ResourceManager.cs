using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : MonoBehaviour {

    public enum ResourceType { Wood, Metal };
    int[] resourceCounts;

    public Transform resourceInventoryParent;
    public GameObject resourcePrefab;

    [Header ("Resource Icons")]
    public Sprite[] icons;

    private void Awake () {
        resourceCounts = new int[System.Enum.GetValues (typeof (ResourceType)).Length];
        for (int i = 0; i < resourceCounts.Length; i++) resourceCounts[i] = 0;
        UpdateDisplay ();
    }

    public void Add (int amount, ResourceType type) {
        resourceCounts[(int) type] += amount;
        UpdateDisplay ();
    }

    public bool Remove (int amount, ResourceType type) {
        if (resourceCounts[(int) type] >= amount) { Add (-amount, type); return true; } else return false;
        // update display is dealt with in the Add function
    }

    public void UpdateDisplay () {
        resourceInventoryParent.gameObject.SetActive (!inventoryEmpty);
        // clear any children
        for (int i = 0; i < resourceInventoryParent.childCount; i++) Destroy (resourceInventoryParent.GetChild (i).gameObject);
        // instantiate new resources
        for(int i = 0; i < resourceCounts.Length; i++) {
            if(resourceCounts[i] > 0) {
                GameObject g = Instantiate (resourcePrefab, resourceInventoryParent);
                g.GetComponent<RequiredResource> ().Setup (icons[i], resourceCounts[i]);
            }
        }
    }

    bool inventoryEmpty {
        get {
            for (int i = 0; i < resourceCounts.Length; i++)
                if (resourceCounts[i] != 0)
                    return false;
            return true;
        }
    }
}