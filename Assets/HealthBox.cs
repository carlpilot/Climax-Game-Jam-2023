using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBox : MonoBehaviour
{
    public float health = 50;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            GetComponentInParent<PlayerHealth>().TakeDamage(-health);
            GetComponentInParent<PlayerInventory>().EmptyCurrentHotbarSlot();
        }
    }
}
