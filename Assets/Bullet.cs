using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public GameObject hitParticlesPrefab;
    void OnCollisionEnter(Collision collision)
    {
        var p = Instantiate(hitParticlesPrefab, transform.position, Quaternion.identity);
        p.SetActive(true);
        Destroy(gameObject);
    }
}
