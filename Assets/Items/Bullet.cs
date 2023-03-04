using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public GameObject hitParticlesPrefab;
    public float directDamage = 20f;
    public float knockbackForce = 10f;
    void OnTriggerEnter(Collider col)
    {
        var p = Instantiate(hitParticlesPrefab, transform.position, Quaternion.identity);
        p.SetActive(true);
        gameObject.SetActive(false);
        var enemy = col.gameObject.GetComponent<Enemy>();
        if (enemy)
        {
            enemy.TakeDamage(directDamage);
            enemy.Knockback(transform.forward, knockbackForce);
        }
        Destroy(gameObject);
    }
}
