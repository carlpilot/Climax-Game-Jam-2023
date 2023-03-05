using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoBox : MonoBehaviour
{
    public int ammo = 10;
    public string ammoType = "Pistol";

    void Awake()
    {
        
    }
    
    void Start()
    {
        transform.Rotate(0, Random.Range(0, 360), 0, Space.World);
    }
    
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        var inv = other.gameObject.GetComponent<PlayerInventory>();
        if (inv)
        {
            inv.AddAmmo(ammoType, ammo);
            Destroy(gameObject);
        }
    }
}
