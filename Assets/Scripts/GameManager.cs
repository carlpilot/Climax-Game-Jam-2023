using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour {

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
    public float timeToSurvive;

    [Header ("Misc")]
    public GameObject pauseMenu;
    public TMP_Text daysSurvived;

    float startIntensity;

    int numWavesTonight;
    bool[,] wavesTonight; // [tonight's wave #, EnemyWave index]

    bool dayIsOver = false;

    public static bool isCurrentlyDay;

    GameObject player;
    Construction con;
    MazeMaker mm;

    public bool isPaused { get; private set; } = false;

    private void Awake () {
        if (startingSeed == -1) startingSeed = Random.Range (int.MinValue + 1, int.MaxValue - 100000);

        mm = FindObjectOfType<MazeMaker> ();
        con = FindObjectOfType<Construction> ();
        player = FindObjectOfType<CharacterController> ().gameObject;

        startIntensity = sun.intensity;

        UpdateMazeProperties ();
        mm.GenerateWorld ();
        OnDayStart ();
    }

    private void Update () {
        currentTime += Time.deltaTime;
        isCurrentlyDay = currentTime < dayLength;
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

        if (Input.GetKeyDown (KeyCode.Escape)) {
            if (!isPaused) TryPause (); else Unpause ();
        }
    }

    public void TryPause () {
        if (mm.rebuildInProgress || con.isPlacing || con.buildMenuOpen) return;
        pauseMenu.SetActive (true);
        daysSurvived.text = "Days Survived: " + currentDay;
        isPaused = true;
        Time.timeScale = 0;
    }

    public void Unpause () {
        pauseMenu.SetActive (false);
        isPaused = false;
        Time.timeScale = 1;
    }

    public void BackToMainMenu () => SceneManager.LoadScene (0);

    public void NextDay () {
        con.CancelPlace ();
        currentDay++;
        currentTime = 0.0f;
        dayIsOver = false;
        StartCoroutine(KillAllEnemies ());
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
            for (int i = 0; i < numWavesTonight; i++) {
                bool isPresentInWave_i = i >= (numWavesTonight - numRecurrences);
                wavesTonight[i, j] = isPresentInWave_i;
            }
        }

        // print tonight's wave info
        /*
        Debug.Log ("Day " + currentDay + ": There will be " + numWavesTonight + " waves tonight");
        for(int i = 0; i < numWavesTonight; i++) {
            Debug.Log ("   Wave " + i);
            for(int j = 0; j < enemies.Length; j++) {
                if (wavesTonight[i, j]) Debug.Log ("    - " + enemies[j].name + ": " + enemies[j].Count (currentDay));
            }
        }*/
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
            if (i != numWavesTonight - 1) yield return new WaitForSeconds (timeBetweenWaves);
        }
        StartCoroutine (WaitToFinishDay ());
    }

    IEnumerator WaitToFinishDay () {
        // go to the next day when either all enemies are dead or the player has survived a specified amount of time
        for(int i = 0; i < timeToSurvive; i++) {
            yield return new WaitForSeconds (1.0f);
            if (GameObject.FindGameObjectsWithTag ("Enemy").Length == 0) {
                Debug.Log ("All enemies killed");
                yield return new WaitForSeconds (2.0f); // bit of an extra wait after killing last enemy
                break;
            }
        }
        NextDay ();
    }

    void SpawnWaveOnce (EnemyWave w) {
        int numToSpawn = w.Count (currentDay);
        Debug.Log ("Spawning wave of " + numToSpawn + " " + w.name);
        for(int i = 0; i < numToSpawn; i++) {
            Vector3 spawnPosition = Vector3.zero;
            int attempts = 0;
            // find a valid spawn position (with max. attempts cap)
            while (!SpawnPositionValid (spawnPosition) && attempts < 100) {
                Vector2 circle = Random.insideUnitCircle.normalized * mm.chunkSize / 2f;
                spawnPosition = player.transform.position + new Vector3 (circle.x, Random.Range(10f, 15f), circle.y);
                attempts++;
            }
            Instantiate (w.prefab, spawnPosition, Quaternion.identity);
        }
    }

    public bool SpawnPositionValid (Vector3 spawnPosition) {
        Vector2Int chunk = mm.WorldPosToChunkNum (spawnPosition);
        return spawnPosition != Vector3.zero && Mathf.Abs(chunk.x) <= mm.worldRadiusChunks && Mathf.Abs(chunk.y) <= mm.worldRadiusChunks;
    }

    public IEnumerator KillAllEnemies () {
        yield return new WaitForSeconds (5f);
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
        return Mathf.RoundToInt (Mathf.Lerp (countMin, countMax, (float) (day - firstNight) / daysToReachMax));
    }
    public int Recurrences (int day) {
        if (day < firstNight) return 0;
        return Mathf.RoundToInt (Mathf.Lerp (recurrencesMin, recurrencesMax, (float) (day - firstNight) / daysToReachMax));
    }
}
