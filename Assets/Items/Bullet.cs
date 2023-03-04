using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public GameObject hitParticlesPrefab;
    public float directDamage = 20f;
    public float knockbackForce = 10f;

    void Update()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, 0.1f);
        foreach (var col in hitColliders)
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
            var player = col.gameObject.GetComponent<PlayerHealth>();
            if (player)
            {
                player.TakeDamage(directDamage);
            }
        }
        if (hitColliders.Length > 0)
        {
            Destroy(gameObject);
        }
    }
}
