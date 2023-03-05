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

    PlayerInventory inventory;

    public string ammoType = "Pistol";

    void Awake()
    {
        animator = GetComponent<Animator>();
        if (isPlayerGun) inventory = GetComponentInParent<PlayerInventory>();
    }
    
    void Start()
    {
        
    }
    
    void Update()
    {
        var canShoot = animator.GetCurrentAnimatorStateInfo(0).IsName("Idle");
        var playershoot = ((Input.GetMouseButtonDown(0) && !isFullAuto) || (Input.GetMouseButton(0) && isFullAuto));
        if (isPlayerGun) playershoot = playershoot && inventory.HasAmmo(ammoType);
        if (canShoot && ((playershoot && isPlayerGun) || (aiIsAttacking && !isPlayerGun)))
        {
            animator.Play("Shoot");
            StartCoroutine(shootAfter(shootDelay));
            if (isPlayerGun) inventory.ConsumeAmmo(ammoType);
        }
    }

    IEnumerator shootAfter(float delay)
    {
        yield return new WaitForSeconds(delay);
        var spreadVel = Vector3.Project(Random.insideUnitSphere, muzzle.right) * spread;
        var bullet = Instantiate(bulletPrefab, muzzle.position, muzzle.rotation);
        bullet.transform.forward = muzzle.forward + spreadVel;
        bullet.GetComponent<Bullet>().speed = bulletSpeed;
        bullet.GetComponent<Bullet>().isEnemyBullet = !isPlayerGun;
    }
}
