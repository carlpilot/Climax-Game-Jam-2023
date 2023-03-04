using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class Item : ScriptableObject
{
    [Header("Item Info")]
    public string itemName;
    public Sprite itemImage;

    [Header("Pickup")]
    public GameObject pickupPrefab;

    [Header("Types (pick one)")]
    public Gun gun;
    public Sword sword;
    public ToolBox toolbox;
}
