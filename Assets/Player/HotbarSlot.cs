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

    public void SetOutlineHighlight(bool highlight)
    {
        outline.enabled = highlight;
    }
}
