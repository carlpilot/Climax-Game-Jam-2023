using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public float health = 100;
    void Awake()
    {
        
    }
    
    void Start()
    {
        
    }
    
    void Update()
    {
        
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0)
        {
            //Destroy(gameObject);
            print("Get oofed");
        }
    }
}
