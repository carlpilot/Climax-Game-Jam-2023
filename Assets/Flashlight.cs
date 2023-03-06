using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flashlight : MonoBehaviour
{

    void Awake()
    {
        
    }
    
    void Start()
    {
        
    }
    
    void Update()
    {
        transform.GetChild(0).gameObject.SetActive(!GameManager.isCurrentlyDay);
    }
}
