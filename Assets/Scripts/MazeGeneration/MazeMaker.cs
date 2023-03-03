using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeMaker : MonoBehaviour {

    public float chunkSize { get => passagewayWidth * chunkNumCells; }

    [Header ("World Settings")]
    public int seed = -1;
    public float passagewayWidth;
    public int chunkNumCells;

    [Header ("Generation Options")]
    [Range (0f, 1f)]
    public float chanceOfGoingStraight;
    [Range (0f, 1f)]
    public float chanceOfDeletingWall;

    [Header ("Materials")]
    public Material floorMaterial;

    Mesh floorMesh;

    private void Start () {
        if (seed == -1) seed = Random.Range (int.MinValue + 1, int.MaxValue);

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
        MazeChunk mc = chunk.AddComponent<MazeChunk> ();
        mc.mm = this;
        mc.chunkNum = chunkNum;
        mc.Generate ();
    }

    public Mesh GetFloorMesh () {
        if (floorMesh != null) return floorMesh;
        Mesh m = new Mesh ();
        m.name = "Floor Mesh";

        Vector3[] verts = new Vector3[(chunkNumCells + 1) * (chunkNumCells + 1)];
        int[] tris = new int[chunkNumCells * chunkNumCells * 6];
        Vector2[] uvs = new Vector2[verts.Length];
        int t = 0;

        for (int i = 0; i <= chunkNumCells; i++) {
            for (int j = 0; j <= chunkNumCells; j++) {
                int idx = i + j * (chunkNumCells + 1);
                verts[idx] = (Vector3.right * (i / (float) chunkNumCells - 0.5f) + Vector3.forward * (j / (float) chunkNumCells - 0.5f)) * chunkSize;
                uvs[idx] = (Vector2.right * i / (float) chunkNumCells + Vector2.up * j / (float) chunkNumCells);
                if (i != chunkNumCells && j != chunkNumCells) {
                    tris[t] = idx + 0;
                    tris[t + 1] = idx + chunkNumCells + 1;
                    tris[t + 2] = idx + 1;
                    tris[t + 3] = idx + chunkNumCells + 1;
                    tris[t + 4] = idx + chunkNumCells + 2;
                    tris[t + 5] = idx + 1;
                    t += 6;
                }
            }
        }

        m.vertices = verts;
        m.triangles = tris;
        m.uv = uvs;
        m.RecalculateNormals ();
        m.RecalculateBounds ();

        floorMesh = m;
        return m;
    }
}