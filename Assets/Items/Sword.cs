using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : MonoBehaviour
{
    Animator animator;

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
            }
        }
        animator.SetBool("isBlocking", Input.GetMouseButton(1));
        isBlocking = animator.GetCurrentAnimatorStateInfo(0).IsName("Block");
    }
}
