using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public GameObject hitParticlesPrefab;
    public float directDamage = 20f;
    public float knockbackForce = 10f;
    public LayerMask hitLayerMask;
    [HideInInspector]
    public float speed = 10f;

    public bool isEnemyBullet = false;
    
    void Start()
    {
        StartCoroutine(DestroyAfter(20f));
    }
    IEnumerator DestroyAfter(float time){
        yield return new WaitForSeconds(time);
        Destroy(gameObject);
    }

    void Update()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, 0.2f, hitLayerMask);
        if (hitColliders.Length > 0)
        {
            var col = hitColliders[0];
            var p = Instantiate(hitParticlesPrefab, transform.position, Quaternion.identity);
            p.SetActive(true);
            gameObject.SetActive(false);
            p.GetComponentInChildren<AudioSource>().Play();
            var enemy = col.gameObject.GetComponent<Enemy>();
            if (enemy)
            {
                if(!isEnemyBullet) enemy.TakeDamage(directDamage);
                enemy.Knockback(transform.forward, knockbackForce);
                Destroy(gameObject);
            }
            var player = col.gameObject.GetComponent<PlayerHealth>();
            if (player)
            {
                var sword = player.GetComponentInChildren<Sword>();
                if (sword && sword.isBlocking){
                    transform.forward = -transform.forward;
                    var rb = GetComponent<Rigidbody>();
                    rb.velocity = -rb.velocity;
                    transform.Translate(Vector3.forward * 0.15f, Space.Self);
                    p.SetActive(false);
                    gameObject.SetActive(true);
                    isEnemyBullet = false;
                    sword.ReduceDurability();
                    sword.GetComponent<Animator>().Play("BlockBounce");
                } else{
                    player.TakeDamage(directDamage);
                    Destroy(gameObject);
                }
            }
        }
        transform.Translate(Vector3.forward * speed * Time.deltaTime, Space.Self);
    }
    
}
