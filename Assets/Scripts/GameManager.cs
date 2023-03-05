using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    [Header ("Timekeeping")]
    public int currentDay = 0;
    public float dayLength;
    public bool forceNextDay = false;
    public Light sun;
    public AnimationCurve sunAngle;
    public AnimationCurve sunIntensity;

    [Header ("Maze Settings")]
    public int startingSeed = -1;
    public AnimationCurve wallRemovalFalloff;
    public float startRemovalFactor;
    public int daysToMaxFalloff;

    [Header ("Enemies")]
    public EnemyWave[] waves;

    GameObject player;
    MazeMaker mm;

    private void Awake () {
        if (startingSeed == -1) startingSeed = Random.Range (int.MinValue + 1, int.MaxValue - 100000);
        mm = FindObjectOfType<MazeMaker> ();
        player = FindObjectOfType<CharacterController> ().gameObject;
        UpdateMazeProperties ();
        mm.GenerateWorld ();
    }

    private void Update () {
        if(forceNextDay) {
            NextDay ();
            forceNextDay = false;
        }
    }

    public void NextDay () {
        currentDay++;
        KillAllEnemies ();
        UpdateMazeProperties ();
        mm.RegenerateWorld ();
    }

    public void UpdateMazeProperties () {
        mm.seed = startingSeed + currentDay;
        mm.chanceOfDeletingWall = GetRemovalFactor (currentDay);
    }

    public void SpawnWave (EnemyWave w) {
        int numToSpawn = w.NumToSpawn (currentDay);
        for(int i = 0; i < numToSpawn; i++) {
            Vector3 spawnPosition = Vector3.zero;
            int attempts = 0;
            // find a valid spawn position (with max. attempts cap)
            while (!SpawnPositionValid (spawnPosition) && attempts < 100) {
                Vector2 circle = Random.insideUnitCircle.normalized * mm.chunkSize / 2f;
                spawnPosition = player.transform.position + new Vector3 (circle.x, 0f, circle.y);
                attempts++;
            }
            Instantiate (w.prefab, spawnPosition, Quaternion.identity);
        }
    }

    public bool SpawnPositionValid (Vector3 spawnPosition) {
        Vector2Int chunk = mm.WorldPosToChunkNum (spawnPosition);
        return spawnPosition != Vector3.zero && Mathf.Abs(chunk.x) <= mm.worldRadiusChunks && Mathf.Abs(chunk.y) <= mm.worldRadiusChunks;
    }

    public void KillAllEnemies () {
        foreach (Enemy e in FindObjectsOfType<Enemy> ()) Destroy (e.gameObject); // bypass Enemy.Die() so there are no drops or effects
    }

    public float GetRemovalFactor(int day) {
        return wallRemovalFalloff.Evaluate ((float) day / daysToMaxFalloff) * startRemovalFactor;
    }
}

[System.Serializable]
public struct EnemyWave {

    public string name;
    public GameObject prefab;
    public int numToSpawnMin;
    public int numToSpawnMax;
    public int numTimesToRecurMin;
    public int numTimesToRecurMax;
    public int daysToReachMax;
    public int firstNightToOccur;

    public int NumToSpawn (int day) {
        return Mathf.RoundToInt (Mathf.Lerp (numToSpawnMin, numToSpawnMax, (float) (day - daysToReachMax) / daysToReachMax));
    }
    public int NumTimesToRecur (int day) {
        return Mathf.RoundToInt (Mathf.Lerp (numTimesToRecurMin, numTimesToRecurMax, (float) (day - daysToReachMax) / daysToReachMax));
    }
}
