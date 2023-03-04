using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnManager : MonoBehaviour
{
    float time;
    public float dayDuration = 60f;
    public GameObject light;
    void Awake()
    {
        
    }
    
    void Start()
    {
        
    }
    
    void Update()
    {
        var degrees = time/dayDuration * Mathf.PI * 2f;
        var currentBrightness = Mathf.Sin(degrees);
        light.transform.rotation = Quaternion.Euler(0, 0, degrees * 180f);
    }
}
