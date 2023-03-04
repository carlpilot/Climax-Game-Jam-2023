using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public float spawnRadiusMin = 30f;
    public float spawnRadiusMax = 60f;
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
            var dist = Random.Range(spawnRadiusMin, spawnRadiusMax);
            var angle = Random.Range(0, Mathf.PI*2);
            var pos = new Vector3(dist*Mathf.Sin(angle), 0, dist*Mathf.Cos(angle));
            var enemy = Instantiate(enemyPrefab, pos+transform.position, Quaternion.identity);
            enemy.SetActive(true);
        }
    }
}
