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
        yield return new WaitForSeconds(force/5f);
        rb.isKinematic = true;
        agent.enabled = true;
    }
}
