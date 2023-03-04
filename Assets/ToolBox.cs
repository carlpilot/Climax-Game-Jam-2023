using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolBox : MonoBehaviour
{
    public bool isHealth = true;
    public float health = 50;

    public bool isShift = false;

    void Update()
    {
        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
        {
            if (isHealth){
                GetComponentInParent<PlayerHealth>().TakeDamage(-health);
            } else if (isShift) {
                print("SHIFT!!!");
            }
            GetComponentInParent<PlayerInventory>().EmptyCurrentHotbarSlot();
        }
    }
}
