using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : MonoBehaviour
{
    Animator animator;
    public float range = 2f;
    public float damage = 60f;
    public float angle = 45f;
    public float knockback = 30f;

    public float damageDelay = 0.15f;

    public bool isBlocking {get; private set;}
    void Awake()
    {
        animator = GetComponent<Animator>();
    }
    
    void Start()
    {
        
    }
    
    void Update()
    {
        // Check if the animator is in the idle state
        if(animator.GetCurrentAnimatorStateInfo(0).IsName("Idle")){
            if(Input.GetMouseButtonDown(0)){
                animator.Play("Swing");
                // Damage all enemies within range
                foreach (var enemy in GameObject.FindObjectsOfType<Enemy>())
                {
                    var swordPos = transform.position-transform.forward;
                    if (Vector3.Distance(transform.position, enemy.transform.position) < range && Vector3.Angle(transform.forward, enemy.transform.position - swordPos) < angle)
                    {
                        StartCoroutine(TakeDamageAfter(damageDelay, damage, enemy, enemy.transform.position - swordPos));
                    }
                }
            }
        }
        animator.SetBool("isBlocking", Input.GetMouseButton(1));
        isBlocking = animator.GetCurrentAnimatorStateInfo(0).IsName("Block");

    }

    IEnumerator TakeDamageAfter(float delay, float damage, Enemy enemy, Vector3 dir)
    {
        yield return new WaitForSeconds(delay);
        enemy.TakeDamage(damage);
        enemy.Knockback(dir.normalized, knockback);
    }
}
