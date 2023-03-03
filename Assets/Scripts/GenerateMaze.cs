using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateMaze : MonoBehaviour {

    public float chunkSize { get => passagewayWidth * chunkNumCells; }

    public float passagewayWidth;
    public int chunkNumCells;

    [Range (0f, 1f)]
    public float chanceOfGoingStraight;
    [Range (0f, 1f)]
    public float chanceOfDeletingWall;

    private void Start () {
        for (int i = -5; i <= 5; i++) {
            for (int j = -5; j <= 5; j++) {
                GenerateChunk (new Vector2Int (i, j));
            }
        }
    }

    public void GenerateChunk (Vector2Int chunkNum) {
        Vector3 worldPosition = new Vector3 (chunkNum.x * chunkSize, 0f, chunkNum.y * chunkSize);
        GameObject chunk = new GameObject ("Chunk " + chunkNum);
        chunk.transform.position = worldPosition;
    }
}
