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

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }
    
    void Start()
    {
        player = GameObject.FindGameObjectWithTag(playerTag).GetComponentInChildren<PlayerMovement>().gameObject;
    }
    
    void Update()
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
