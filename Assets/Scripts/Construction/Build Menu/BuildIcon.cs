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

    public void Assign (Buildable b) {
        menu = FindObjectOfType<BuildMenu> ();
        icon.sprite = b.icon;
        title.text = b.name;

        if (b.woodRequired > 0) {
            RequiredResource r = Instantiate (menu.resourcePrefab, resourceParent).GetComponent<RequiredResource> ();
            r.Setup (menu.woodIcon, b.woodRequired);
        }

        if (b.metalRequired > 0) {
            RequiredResource r = Instantiate (menu.resourcePrefab, resourceParent).GetComponent<RequiredResource> ();
            r.Setup (menu.metalIcon, b.metalRequired);
        }
    }
}
