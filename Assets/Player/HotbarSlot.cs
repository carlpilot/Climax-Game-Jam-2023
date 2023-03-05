using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HotbarSlot : MonoBehaviour
{
    public Image icon;
    public Image outline;
    public TextMeshProUGUI index;
    public Image bg;

    Color initColor;

    void Awake()
    {
        initColor = bg.color;
    }

    float durabilityPercent;

    public void SetOutlineHighlight(bool highlight)
    {
        outline.enabled = highlight;
    }

    public void SetDurability(float percent)
    {
        durabilityPercent = percent;
        //icon.color = Color.Lerp(Color.red, Color.white, percent);
        bg.color = Color.Lerp(Color.red, initColor, percent);
    }
}
