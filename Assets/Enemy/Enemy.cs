using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public string playerTag;
    GameObject player;
    NavMeshAgent agent;

    public float attackRange = 1.0f;

    public bool isAttacking {get; private set;}

    public float health = 100.0f;

    Rigidbody rb;

    public float recoverySpeed = 0.1f;


    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();
    }
    
    void Start()
    {
        player = GameObject.FindGameObjectWithTag(playerTag).GetComponentInChildren<PlayerMovement>().gameObject;
    }
    
    void Update()
    {
        if (rb.isKinematic)
        {
            if (Vector3.Distance(transform.position, player.transform.position) < attackRange)
            {
                // Stop the navmesh agent
                agent.isStopped = true;
                isAttacking = true;
            } else{
                // Start the navmesh agent
                agent.isStopped = false;
                agent.SetDestination(player.transform.position);
                isAttacking = false;
            }
        }
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }

    public void Knockback(Vector3 direction, float force)
    {
        rb.isKinematic = false;
        agent.enabled = false;
        StartCoroutine(KnockbackNextFrame(direction, force));
    }

    IEnumerator KnockbackNextFrame(Vector3 direction, float force)
    {
        yield return new WaitForEndOfFrame();
        rb.velocity = direction * force;
        while (rb.velocity.magnitude > recoverySpeed)
        {
            yield return null;
        }
        rb.isKinematic = true;
        agent.enabled = true;
    }

    // Check if we collide with any other enemies
    void OnCollisionEnter(Collision col)
    {
        var enemy = col.gameObject.GetComponent<Enemy>();
        if (enemy)
        {
            // Get the direction to the other enemy
            Vector3 direction = (col.gameObject.transform.position - transform.position).normalized;
            // Apply the knockback
            enemy.Knockback(direction, 10.0f);
        }
    }
}
