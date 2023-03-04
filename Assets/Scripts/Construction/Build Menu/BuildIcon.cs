using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuildIcon : MonoBehaviour {

    BuildMenu menu;

    public Transform resourceParent;
    public Image icon;
    public TMP_Text title;

    Buildable b;

    public void Assign (Buildable buildable) {
        b = buildable;

        menu = FindObjectOfType<BuildMenu> ();
        icon.sprite = b.icon;
        title.text = b.name;

        if (b.woodRequired > 0) {
            RequiredResource r = Instantiate (menu.resourcePrefab, resourceParent).GetComponent<RequiredResource> ();
            r.Setup (menu.rm.icons[(int) ResourceManager.ResourceType.Wood], b.woodRequired);
        }

        if (b.metalRequired > 0) {
            RequiredResource r = Instantiate (menu.resourcePrefab, resourceParent).GetComponent<RequiredResource> ();
            r.Setup (menu.rm.icons[(int) ResourceManager.ResourceType.Metal], b.metalRequired);
        }
    }

    public void Select () {
        Debug.Log ("Selected " + b.name);
        Construction.inst.SelectBuildable (b);
    }
}
