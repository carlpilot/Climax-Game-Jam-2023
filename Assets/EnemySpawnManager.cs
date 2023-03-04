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
        time = dayDuration/4f*2.1f;
    }
    
    void Start()
    {
        
    }
    
    void Update()
    {
        var rads = time/dayDuration * Mathf.PI * 2f;
        var degs = rads/Mathf.PI * 180f;
        var currentBrightness = Mathf.Sin(rads);
        light.transform.rotation = Quaternion.Euler(-degs, 30, 0);
        time += Time.deltaTime;
    }
}
