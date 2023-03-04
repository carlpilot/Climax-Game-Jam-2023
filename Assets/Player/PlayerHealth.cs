using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public float health {get; private set;}
    public float maxHealth = 500;
    public HealthBar bar;
    void Awake()
    {
        health = maxHealth;
    }
    
    void Start()
    {
        bar.SetHealth(health, maxHealth);
    }
    
    void Update()
    {
        
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0)
        {
            health = 0;
            //Destroy(gameObject);
            print("Get oofed");
        }
        bar.SetHealth(health, maxHealth);
    }
}
