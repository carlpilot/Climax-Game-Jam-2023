using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RequiredResource : MonoBehaviour {

    public Image icon;
    public TMP_Text quantityText;

    public void Setup (Sprite sprite, int quantity) {
        icon.sprite = sprite;
        quantityText.text = "x " + quantity;
    }

}
