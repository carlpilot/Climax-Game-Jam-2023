using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnManager : MonoBehaviour
{
    float time;
    public float dayDuration = 60f;
    public GameObject light;

    public float mapSize = 150f;
    public float enemySpawnRadius = 30f;
    public float enemySpawnMaxRadius = 80f;
    public float enemySpawnCentralRadius = 30f;

    int waveCount = 0;
    public float currentSpawnDelay = 10f;
    public float currentWaveCount = 2f;
    float lastWaveTimer;

    public GameObject[] enemyPrefabs;
    public float[] enemySpawnChances;

    GameObject player;

    void Awake()
    {
        time = dayDuration/4f*2.1f;
        player = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<PlayerHealth>().gameObject;
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
                    SpawnEnemy(transform.position);
                }
            }
        } else{
            // Day time
            // Spawn enemies slowly around the player
            if (lastWaveTimer <= 0){
                lastWaveTimer = currentSpawnDelay;
                SpawnEnemy(transform.position);
            }
        }
        lastWaveTimer -= Time.deltaTime;
    }

    void SpawnEnemy(Vector3 center){
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
                var enemy = Instantiate(enemyPrefabs[i], PickSpawnPosition(), Quaternion.identity);
                enemy.SetActive(true);
                return;
            }
        }
    }

    Vector3 PickSpawnPosition(){
        var pos = new Vector3(Random.Range(-mapSize, mapSize), 0, Random.Range(-mapSize, mapSize));
        // Cant spawn in the center
        if (Vector3.Distance(pos, transform.position) < enemySpawnCentralRadius){
            return PickSpawnPosition();
        }
        var d = Vector3.Distance(pos, player.transform.position);
        if (d < enemySpawnRadius || d > enemySpawnMaxRadius){
            return PickSpawnPosition();
        }
        return pos;
    }
}
