using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public Transform muzzle;
    public GameObject bulletPrefab;
    public float bulletSpeed = 10f;

    public bool isFullAuto = false;

    Animator animator;

    public float shootDelay = 0;

    public float spread = 0.1f;

    public bool isPlayerGun = true;

    public bool aiIsAttacking = false;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }
    
    void Start()
    {
        
    }
    
    void Update()
    {
        var canShoot = animator.GetCurrentAnimatorStateInfo(0).IsName("Idle");
        var playershoot = ((Input.GetMouseButtonDown(0) && !isFullAuto) || (Input.GetMouseButton(0) && isFullAuto));
        if (canShoot && ((playershoot && isPlayerGun) || (aiIsAttacking && !isPlayerGun)))
        {
            animator.Play("Shoot");
            StartCoroutine(shootAfter(shootDelay));
        }
    }

    IEnumerator shootAfter(float delay)
    {
        yield return new WaitForSeconds(delay);
        var spreadVel = Vector3.ProjectOnPlane(Random.insideUnitSphere, muzzle.forward).normalized * spread;
        var bullet = Instantiate(bulletPrefab, muzzle.position, muzzle.rotation);
        bullet.GetComponent<Rigidbody>().velocity = bullet.transform.forward * bulletSpeed+spreadVel;
    }
}
