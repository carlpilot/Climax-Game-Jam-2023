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

    int waveCount = 0;
    public float currentSpawnDelay = 10f;
    public float currentWaveCount = 2f;
    float lastWaveTimer;

    public GameObject[] enemyPrefabs;
    public float[] enemySpawnChances;

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
                lastWaveTimer = 20f;
                waveCount += 1;
                currentWaveCount *= 1.2f;
                currentSpawnDelay *= 0.9f;
                var numEnemies = Mathf.Pow(waveCount/5f, 2);
                for (int i = 0; i < Mathf.CeilToInt(currentWaveCount); i++)
                {
                    SpawnEnemy(transform.position, nighttimeSpawnRadius);
                }
            }
        } else{
            // Day time
            // Spawn enemies slowly around the player
            if (lastWaveTimer <= 0){
                lastWaveTimer = currentSpawnDelay;
                SpawnEnemy(transform.position, nighttimeSpawnRadius);
            }
        }
        lastWaveTimer -= Time.deltaTime;
    }

    void SpawnEnemy(Vector3 center, float range){
        // Pick a random enemy to spawn with roulette wheel selection
        var totalChance = 0f;
        foreach (var chance in enemySpawnChances)
        {
            totalChance += chance;
        }
        var rand = Random.Range(0f, totalChance);
        var currentChance = 0f;
        for (int i = 0; i < enemySpawnChances.Length; i++)
        {
            currentChance += enemySpawnChances[i];
            if (rand <= currentChance){
                var pos = Random.insideUnitSphere;
                pos.y = 0;
                pos = pos.normalized * range;
                var enemy = Instantiate(enemyPrefabs[i], pos+center, Quaternion.identity);
                enemy.SetActive(true);
                return;
            }
        }
    }
}
