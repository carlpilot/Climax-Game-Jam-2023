using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.UI;

public class MazeMaker : MonoBehaviour {

    public float chunkSize { get => passagewayWidth * chunkNumCells; }

    [Header ("World Settings")]
    public int seed = -1;
    public float passagewayWidth;
    public int chunkNumCells;
    public int worldRadiusChunks;
    public bool forceRegenerate = false;

    [Header ("Generation Options")]
    [Range (0f, 1f)]
    public float chanceOfDeletingWall;
    public float wallHeight = 2.0f;
    public float pillarHeight = 2.5f;
    public GameObject navMeshPrefab;

    [Header ("Visual")]
    public Material floorMaterial;
    public Material wallMaterial;
    public Material pillarMaterial;
    public Mesh wallMesh;
    public Mesh pillarMesh;

    [Header ("UI")]
    public GameObject mazeShiftNotice;
    public Slider mazeShiftProgress;

    [Header ("Resources")]
    public ResourceChance[] resourceChances;

    public bool rebuildInProgress { get; private set; } = false;

    Mesh floorMesh;
    int floorMeshRes = 0;
    float floorPW = 0f;

    public Dictionary<Vector2Int, GameObject> chunks = new Dictionary<Vector2Int, GameObject> ();

    GameManager gm;

    private void Awake () {
        gm = FindObjectOfType<GameManager> ();
    }

    private void Start () {
        if (seed == -1) seed = Random.Range (int.MinValue + 1, int.MaxValue - 100000);
        if (gm == null) GenerateWorld ();
    }

    private void Update () {
        if (forceRegenerate) {
            RegenerateWorld ();
            forceRegenerate = false;
        }
    }

    public void GenerateWorld () {
        for (int i = -worldRadiusChunks; i <= worldRadiusChunks; i++) {
            for (int j = -worldRadiusChunks; j <= worldRadiusChunks; j++) {
                GenerateChunk (new Vector2Int (i, j));
            }
        }
        // FindObjectOfType<NavMeshSurface> ().BuildNavMesh ();
    }

    public void StepGenerateWorld () {
        seed++; // randomise the world every time it regenerates
        mazeShiftProgress.value = 0f;
        StartCoroutine (RegenerateCoroutine ());
    }

    IEnumerator RegenerateCoroutine() {
        rebuildInProgress = true;
        Time.timeScale = 0;
        mazeShiftNotice.SetActive (true);
        int numToGenerate = (worldRadiusChunks * 2 + 1) * (worldRadiusChunks * 2 + 1);
        int numGenerated = 0;
        for (int i = -worldRadiusChunks; i <= worldRadiusChunks; i++) {
            for (int j = -worldRadiusChunks; j <= worldRadiusChunks; j++) {
                GenerateChunk (new Vector2Int (i, j));
                numGenerated++;
                mazeShiftProgress.value = (float) numGenerated / numToGenerate;
                yield return new WaitForEndOfFrame ();
            }
        }
        // FindObjectOfType<NavMeshSurface> ().BuildNavMesh ();
        mazeShiftNotice.SetActive (false);
        Time.timeScale = 1;
        rebuildInProgress = false;
    }

    public void GenerateChunk (Vector2Int chunkNum) {
        if (chunks.ContainsKey (chunkNum)) RemoveChunk (chunkNum);
        Vector3 worldPosition = new Vector3 (chunkNum.x, 0f, chunkNum.y) * chunkSize;
        GameObject chunk = new GameObject ("Chunk " + chunkNum);
        chunk.transform.position = worldPosition;

        MazeChunk mc = chunk.AddComponent<MazeChunk> ();
        mc.mm = this;
        mc.chunkNum = chunkNum;
        mc.Generate ();

        chunks[chunkNum] = chunk;
    }

    public void RegenerateWorld () {
        foreach(GameObject g in chunks.Values) {
            Destroy (g);
        }
        chunks = new Dictionary<Vector2Int, GameObject> ();
        StepGenerateWorld ();
    }

    public void RemoveChunk (Vector2Int chunkNum) {
        Destroy (chunks[chunkNum].gameObject);
        chunks.Remove (chunkNum);
    }

    public Mesh GetFloorMesh () {
        if (floorMesh != null && floorMeshRes == chunkNumCells && floorPW == passagewayWidth) return floorMesh;
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
        floorMeshRes = chunkNumCells;
        floorPW = passagewayWidth;
        return m;
    }

    public Vector2Int WorldPosToChunkNum (Vector3 worldPos)
        => new Vector2Int (Mathf.RoundToInt (worldPos.x / chunkSize), Mathf.RoundToInt (worldPos.z / chunkSize));
}

[System.Serializable]
public struct ResourceChance {
    public GameObject prefab;
    public float chance;
}