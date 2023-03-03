using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Item : ScriptableObject
{
    [Header("Item Info")]
    public string itemName;
    public Sprite itemImage;

    [Header("Gun")]
    public bool isGun;
    public GameObject gunPrefab;
    public bool isFullAuto;
    public float fireRate;
    public GameObject bullet;

    [Header("Drugs")]
    public bool isDrug;
    public float drugEffectTime;
    public float speedBoost;
    public float healthBoost;
}
