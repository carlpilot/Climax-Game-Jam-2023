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

    public bool isPlayerSword = true;

    public bool aiIsAttacking = false;

    public TrailRenderer trail;

    public GameObject boom;

    public int durability = 100;

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
            if((Input.GetMouseButtonDown(0) && isPlayerSword) || (aiIsAttacking && !isPlayerSword)){
                animator.Play("Swing");
                if (isPlayerSword){
                    foreach (var enemy in GameObject.FindObjectsOfType<Enemy>())
                    {
                        if (isHittable(enemy.transform))
                        {
                            var swordPos = transform.position-transform.forward;
                            StartCoroutine(TakeDamageAfter(damageDelay, damage, enemy, enemy.transform.position - swordPos));
                        }
                    }
                    ReduceDurability();
                    
                }else{
                    var player = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<PlayerHealth>();
                    if (isHittable(player.transform))
                    {
                        var swordPos = transform.position-transform.forward;
                        StartCoroutine(TakePlayerDamageAfter(damageDelay, damage, player));
                    }
                }
            }
        }
        animator.SetBool("isBlocking", Input.GetMouseButton(1)&&isPlayerSword);
        isBlocking = animator.GetCurrentAnimatorStateInfo(0).IsName("Block") || animator.GetCurrentAnimatorStateInfo(0).IsName("BlockBounce");
        if (trail) trail.emitting = animator.GetCurrentAnimatorStateInfo(0).IsName("Swing");
    }

    IEnumerator TakeDamageAfter(float delay, float damage, Enemy enemy, Vector3 dir)
    {
        yield return new WaitForSeconds(delay);
        enemy.TakeDamage(damage);
        enemy.Knockback(dir.normalized, knockback);
        var b = Instantiate(boom, enemy.transform.position, Quaternion.identity);
        b.SetActive(true);
    }

    IEnumerator TakePlayerDamageAfter(float delay, float damage, PlayerHealth player)
    {
        yield return new WaitForSeconds(delay);
        var sword = player.GetComponentInChildren<Sword>();
        if (sword && sword.isBlocking){
            sword.GetComponent<Animator>().Play("BlockBounce");
            yield break;
        }
        player.TakeDamage(damage);
    }

    bool isHittable(Transform t){
        var swordPos = transform.position-transform.forward;
        return Vector3.Distance(transform.position, t.position) < range && Vector3.Angle(transform.forward, t.position - swordPos) < angle;
    }

    public void ReduceDurability(){
        durability--;
        if (durability <= 0){
            GetComponentInParent<PlayerInventory>().EmptyCurrentHotbarSlot();
        }
    }
}
