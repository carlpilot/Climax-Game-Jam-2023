using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfter : MonoBehaviour
{

    void Awake()
    {
        
    }
    
    void Start()
    {
        StartCoroutine(DestroyAfterT(20f));
    }
    IEnumerator DestroyAfterT(float time){
        yield return new WaitForSeconds(time);
        Destroy(gameObject);
    }
    
    void Update()
    {
        
    }
}
