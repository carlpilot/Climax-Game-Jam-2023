using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour {
    public float health { get; private set; }
    public float maxHealth = 500;
    public HealthBar bar;

    GameManager gm;

    void Awake () {
        health = maxHealth;
        gm = FindObjectOfType<GameManager> ();
    }

    void Start () {
        bar.SetHealth (health, maxHealth);
    }

    public void TakeDamage (float damage) {
        health -= damage;
        if (health <= 0) {
            health = 0;
            gm.Lose ();
        }
        bar.SetHealth (health, maxHealth);
    }
}