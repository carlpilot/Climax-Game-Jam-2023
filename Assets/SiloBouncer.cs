using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SiloBouncer : MonoBehaviour
{

    void Awake()
    {
        
    }
    
    void Start()
    {
        
    }
    
    void Update()
    {
        
    }

    void OnCollisionEnter(Collision collision)
    {
        collision.gameObject.transform.position += new Vector3(5, 0, 0);
    }
}
