using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    [Header ("Timekeeping")]
    public int currentDay = 0;
    public float currentTime = 0f;
    public float dayLength;
    public bool forceNextDay = false;
    public Light sun;
    public AnimationCurve sunAngle;
    public AnimationCurve sunHeading;
    public AnimationCurve sunIntensity;
    public Gradient sunColour;

    [Header ("Maze Settings")]
    public int startingSeed = -1;
    public AnimationCurve wallRemovalFalloff;
    public float startRemovalFactor;
    public int daysToMaxFalloff;

    [Header ("Enemies")]
    public EnemyWave[] enemies;
    public float timeBetweenWaves;

    float startIntensity;

    int numWavesTonight;
    bool[,] wavesTonight; // [tonight's wave #, EnemyWave index]

    bool dayIsOver = false;

    GameObject player;
    MazeMaker mm;

    private void Awake () {
        if (startingSeed == -1) startingSeed = Random.Range (int.MinValue + 1, int.MaxValue - 100000);

        mm = FindObjectOfType<MazeMaker> ();
        player = FindObjectOfType<CharacterController> ().gameObject;

        startIntensity = sun.intensity;

        UpdateMazeProperties ();
        mm.GenerateWorld ();
        OnDayStart ();
    }

    private void Update () {
        currentTime += Time.deltaTime;
        float dayFraction = currentTime / dayLength;

        sun.transform.eulerAngles = new Vector3 (sunAngle.Evaluate (dayFraction), sunHeading.Evaluate(dayFraction), 0f);
        sun.intensity = sunIntensity.Evaluate (dayFraction) * startIntensity;
        sun.color = sunColour.Evaluate (dayFraction);

        if(forceNextDay) {
            NextDay ();
            forceNextDay = false;
        }

        if(!dayIsOver && currentTime > dayLength) {
            dayIsOver = true;
            StartCoroutine (SpawnWaves ());
        }
    }

    public void NextDay () {
        currentDay++;
        currentTime = 0.0f;
        dayIsOver = false;
        KillAllEnemies ();
        UpdateMazeProperties ();
        mm.RegenerateWorld ();
        OnDayStart ();
    }

    void OnDayStart () {
        // calculate which waves include which enemies
        numWavesTonight = 0;
        foreach (EnemyWave w in enemies) numWavesTonight = Mathf.Max (numWavesTonight, w.Recurrences (currentDay));
        wavesTonight = new bool[numWavesTonight, enemies.Length];
        for (int j = 0; j < enemies.Length; j++) {
            int numRecurrences = enemies[j].Recurrences (currentDay);
            for (int i = 0; i < wavesTonight.Length; i++) {
                bool isPresentInWave_i = i >= (numWavesTonight - numRecurrences);
                wavesTonight[i, j] = isPresentInWave_i;
            }
        }
    }

    void UpdateMazeProperties () {
        mm.seed = startingSeed + currentDay;
        mm.chanceOfDeletingWall = GetRemovalFactor (currentDay);
    }

    IEnumerator SpawnWaves () {
        for(int i = 0; i < numWavesTonight; i++) {
            for(int j = 0; j < enemies.Length; j++) {
                if (wavesTonight[i, j]) SpawnWaveOnce (enemies[j]);
            }
            yield return new WaitForSeconds (timeBetweenWaves);
        }
    }

    void SpawnWaveOnce (EnemyWave w) {
        int numToSpawn = w.Count (currentDay);
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
    public int countMin;
    public int countMax;
    public int recurrencesMin;
    public int recurrencesMax;
    public int daysToReachMax;
    public int firstNight;

    public int Count (int day) {
        if (day < firstNight) return 0;
        return Mathf.RoundToInt (Mathf.Lerp (countMin, countMax, (float) (day - daysToReachMax) / daysToReachMax));
    }
    public int Recurrences (int day) {
        if (day < firstNight) return 0;
        return Mathf.RoundToInt (Mathf.Lerp (recurrencesMin, recurrencesMax, (float) (day - daysToReachMax) / daysToReachMax));
    }
}
