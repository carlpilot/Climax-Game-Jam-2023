using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnManager : MonoBehaviour
{
    float time;
    public float dayDuration = 60f;
    public GameObject light;

    public float daytimeSpawnRadius = 30f;
    public float nighttimeSpawnRadius = 50f;

    float waveCount = 0;
    public float currentSpawnDelay = 10f;
    float lastWaveTimer;

    public GameObject enemyPrefab;

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
        var currentBrightness = -Mathf.Sin(rads);
        light.transform.rotation = Quaternion.Euler(-degs, 30, 0);
        time += Time.deltaTime;

        if (currentBrightness < 0){
            // Night time
            // Spawn large waves of enemies around the player
            if (lastWaveTimer <= 0){
                lastWaveTimer = 10f;
                waveCount += 1;
                var numEnemies = Mathf.Pow(waveCount/5f, 2);
                for (int i = 0; i < numEnemies; i++)
                {
                    SpawnEnemy(transform.position, nighttimeSpawnRadius);
                }
            }
        } else{
            // Day time
            // Spawn enemies slowly around the player
            if (lastWaveTimer <= 0){
                lastWaveTimer = (5/(waveCount+1))*5;
                SpawnEnemy(transform.position, nighttimeSpawnRadius);
            }
        }
        lastWaveTimer -= Time.deltaTime;
    }

    void SpawnEnemy(Vector3 center, float range){
        var pos = Random.insideUnitSphere;
        pos.y = 0;
        pos = pos.normalized * range;
        var enemy = Instantiate(enemyPrefab, pos+center, Quaternion.identity);
        enemy.SetActive(true);
    }
}
