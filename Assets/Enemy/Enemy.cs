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
    public float seekRange = 15.0f;

    public bool isAttacking {get; private set;}

    public float health = 100.0f;

    Rigidbody rb;

    public float recoverySpeed = 0.1f;
    public float knockbackForce = 2f;
    public bool enableCollisionChaining = false;

    Sword sword;
    Gun gun;

    public GameObject deathEffect;

    float sineTimer;
    public float shootBurstSpeed = 1f;
    // between -1 and 1
    public float shootBurstThreshold = 0f;


    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();
        sword = GetComponentInChildren<Sword>();
        gun = GetComponentInChildren<Gun>();
    }
    
    void Start()
    {
        sineTimer = Random.Range(0f, 200f);
        player = GameObject.FindGameObjectWithTag(playerTag).GetComponentInChildren<PlayerMovement>().gameObject;
    }
    
    void Update()
    {
        if (rb.isKinematic)
        {
            // If it is daytime and we are not in range of the player
            if (EnemySpawnManager.IsDayTime() && Vector3.Distance(transform.position, player.transform.position) > seekRange)
            {
                agent.isStopped = true;
                isAttacking = false;
            } else{
                if (gun){
                    // Check if we raycast towards the player that we hit the player
                    var canSeePlayer =  (Physics.Raycast(transform.position, player.transform.position - transform.position, out var hit, 100.0f) && hit.collider.gameObject.GetComponent<PlayerHealth>());
                    
                    if (Vector3.Distance(transform.position, player.transform.position) < attackRange && canSeePlayer)
                    {
                        // Stop the navmesh agent
                        agent.isStopped = true;
                        var targetRot = Quaternion.LookRotation(Vector3.ProjectOnPlane(player.transform.position - transform.position-transform.right, Vector3.up));
                        transform.rotation = Quaternion.Lerp(transform.rotation, targetRot, Time.deltaTime);
                    } else{
                        // Start the navmesh agent
                        agent.isStopped = false;
                        agent.SetDestination(player.transform.position);
                    }
                    isAttacking = canSeePlayer && Mathf.Sin(sineTimer) > shootBurstThreshold;
                } else{
                    if (Vector3.Distance(transform.position, player.transform.position) < attackRange)
                    {
                        // Stop the navmesh agent
                        agent.isStopped = true;
                        var targetRot = Quaternion.LookRotation(Vector3.ProjectOnPlane(player.transform.position - transform.position-transform.right, Vector3.up));
                        transform.rotation = Quaternion.Lerp(transform.rotation, targetRot, Time.deltaTime);
                        isAttacking = true;
                    } else{
                        // Start the navmesh agent
                        agent.isStopped = false;
                        agent.SetDestination(player.transform.position);
                        isAttacking = false;
                    }
                } 
            }
        } else{
            isAttacking = false;
        }

        if (sword) sword.aiIsAttacking = isAttacking;
        if (gun) gun.aiIsAttacking = isAttacking;

        sineTimer += Time.deltaTime * shootBurstSpeed;
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0)
        {
            deathEffect.SetActive(true);
            deathEffect.transform.parent = null;
            Destroy(gameObject);
        }
    }

    public void Knockback(Vector3 direction, float force)
    {
        if (rb.isKinematic){
            rb.isKinematic = false;
            agent.enabled = false;
            StartCoroutine(KnockbackNextFrame(direction, force));
        } else{
            rb.velocity = direction * force;
        }
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
        if (!enableCollisionChaining) return;
        var enemy = col.gameObject.GetComponent<Enemy>();
        if (enemy)
        {
            // Get the direction to the other enemy
            Vector3 direction = (col.gameObject.transform.position - transform.position).normalized;
            // Apply the knockback
            enemy.Knockback(direction, knockbackForce);
            
        }
    }
}
