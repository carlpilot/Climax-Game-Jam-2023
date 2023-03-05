using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    [Header ("Timekeeping")]
    public int currentDay = 0;
    public float dayLength;
    public bool forceNextDay = false;

    [Header ("Maze Settings")]
    public int startingSeed = -1;
    public AnimationCurve wallRemovalFalloff;
    public float startRemovalFactor;
    public int daysToMaxFalloff;

    [Header ("Enemies")]
    public GameObject[] enemyPrefabs;

    MazeMaker mm;

    private void Awake () {
        if (startingSeed == -1) startingSeed = Random.Range (int.MinValue + 1, int.MaxValue - 100000);
        mm = FindObjectOfType<MazeMaker> ();
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
        UpdateMazeProperties ();
        mm.RegenerateWorld ();
    }

    public void UpdateMazeProperties () {
        mm.seed = startingSeed + currentDay;
        mm.chanceOfDeletingWall = GetRemovalFactor (currentDay);
    }

    public float GetRemovalFactor(int day) {
        return wallRemovalFalloff.Evaluate ((float) day / daysToMaxFalloff) * startRemovalFactor;
    }
}
