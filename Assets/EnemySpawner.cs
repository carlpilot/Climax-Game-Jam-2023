using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public float spawnRadius = 30f;
    public GameObject enemyPrefab;
    public float spawnDelay = 2f;

    void Awake()
    {
        
    }
    
    void Start()
    {
        StartCoroutine(SpawnEnemyEvery());
    }
    
    void Update()
    {
        
    }

    IEnumerator SpawnEnemyEvery(){
        while (true)
        {
            yield return new WaitForSeconds(spawnDelay);
            var pos = Random.insideUnitSphere;
            pos.y = 0;
            pos = pos.normalized * spawnRadius;
            var enemy = Instantiate(enemyPrefab, pos+transform.position, Quaternion.identity);
            enemy.SetActive(true);
        }
    }
}
