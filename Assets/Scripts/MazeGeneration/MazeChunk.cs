using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class MazeChunk : MonoBehaviour {

    public MazeMaker mm;
    public Vector2Int chunkNum;

    Vector2Int[,] parents;
    bool[,] hWalls;
    bool[,] vWalls;

    Vector3 chunkPos;
    Vector2Int unassigned = new Vector2Int (-1, -1);
    bool solved = false;

    [HideInInspector]
    public GameObject floor, walls, pillars, navFloor;

    public void Generate () {
        chunkPos = new Vector3 (chunkNum.x, 0f, chunkNum.y) * mm.chunkSize;
        CreateFloor ();
        if (chunkNum != Vector2.zero) {
            SolveMaze ();
            CreateWalls ();
            SpawnResources ();
        }
        CreateNavMeshFloor ();
        if (chunkNum != Vector2.zero) {
            LinkNavMeshes ();
        }
    }

    void CreateFloor () {
        floor = new GameObject ("Floor Mesh " + chunkNum);
        floor.transform.parent = transform;
        floor.transform.localPosition = Vector3.zero;
        floor.layer = 6;
        MeshFilter mf = floor.AddComponent<MeshFilter> ();
        mf.mesh = mm.GetFloorMesh ();
        MeshRenderer mr = floor.AddComponent<MeshRenderer> ();
        MeshCollider mc = floor.AddComponent<MeshCollider> ();
        mr.material = mm.floorMaterial;
    }

    void SolveMaze () {
        // Ensure consistent randomness
        Random.InitState (("" + mm.seed + chunkNum).GetHashCode ());

        // Initialise arrays - parents to (-1, -1) and all walls activated
        parents = new Vector2Int[mm.chunkNumCells, mm.chunkNumCells];
        hWalls = new bool[mm.chunkNumCells + 1, mm.chunkNumCells];
        vWalls = new bool[mm.chunkNumCells, mm.chunkNumCells + 1];
        for (int i = 0; i < mm.chunkNumCells + 1; i++) {
            for (int j = 0; j < mm.chunkNumCells + 1; j++) {
                if (i != mm.chunkNumCells && j != mm.chunkNumCells) parents[i, j] = unassigned;
                if (j != mm.chunkNumCells) hWalls[i, j] = true;
                if (i != mm.chunkNumCells) vWalls[i, j] = true;
            }
        }

        // March through the maze, removing walls that are in the way
        Vector2Int activeCell = Vector2Int.zero;
        parents[0, 0] = Vector2Int.zero; // prevent origin cell from being free
        while (true) {
            List<Vector2Int> freeNeighbours = new List<Vector2Int> ();
            if (activeCell.x != 0 && parents[activeCell.x - 1, activeCell.y] == unassigned) freeNeighbours.Add (new Vector2Int (activeCell.x - 1, activeCell.y));
            if (activeCell.y != 0 && parents[activeCell.x, activeCell.y - 1] == unassigned) freeNeighbours.Add (new Vector2Int (activeCell.x, activeCell.y - 1));
            if (activeCell.x != mm.chunkNumCells - 1 && parents[activeCell.x + 1, activeCell.y] == unassigned) freeNeighbours.Add (new Vector2Int (activeCell.x + 1, activeCell.y));
            if (activeCell.y != mm.chunkNumCells - 1 && parents[activeCell.x, activeCell.y + 1] == unassigned) freeNeighbours.Add (new Vector2Int (activeCell.x, activeCell.y + 1));

            if (freeNeighbours.Count == 0) {
                // No free neighbours, backtrack
                if (activeCell == Vector2Int.zero) {
                    // backtracked all the way to the beginning, maze is complete
                    break;
                } else {
                    // backtrack to parent of active cell
                    activeCell = parents[activeCell.x, activeCell.y];
                }
            } else {
                // Make connection
                Vector2Int oldActiveCell = activeCell;
                activeCell = freeNeighbours[Random.Range (0, freeNeighbours.Count)];
                int wallIndexX = Mathf.Max (oldActiveCell.x, activeCell.x);
                int wallIndexZ = Mathf.Max (oldActiveCell.y, activeCell.y);
                // Break wall
                if (activeCell.x != oldActiveCell.x) {
                    // Horizontal wall
                    hWalls[wallIndexX, wallIndexZ] = false;
                } else {
                    // Vertical wall
                    vWalls[wallIndexX, wallIndexZ] = false;
                }
                // set parent
                parents[activeCell.x, activeCell.y] = oldActiveCell;
            }
        }

        // Remove walls at chunk boundaries
        int centre = Mathf.FloorToInt (mm.chunkNumCells / 2f);
        vWalls[centre, 0] = false;
        vWalls[centre, mm.chunkNumCells] = false;
        hWalls[0, centre] = false;
        hWalls[mm.chunkNumCells, centre] = false;

        // Remove random walls
        for (int i = 0; i < hWalls.GetLength (0); i++) {
            for (int j = 0; j < hWalls.GetLength (1); j++) {
                if (Random.value < mm.chanceOfDeletingWall) hWalls[i, j] = false;
            }
        }
        for (int i = 0; i < vWalls.GetLength (0); i++) {
            for (int j = 0; j < vWalls.GetLength (1); j++) {
                if (Random.value < mm.chanceOfDeletingWall) vWalls[i, j] = false;
            }
        }

        solved = true;
    }

    void CreateWalls () {

        // Remove duplicate walls
        if(chunkNum.x > 0 && chunkNum != Vector2Int.right) {
            for (int j = 0; j < hWalls.GetLength (1); j++) hWalls[0, j] = false;
        }
        if(chunkNum.x < 0 && chunkNum != Vector2Int.left) {
            for (int j = 0; j < hWalls.GetLength (1); j++) hWalls[mm.chunkNumCells, j] = false;
        }
        if(chunkNum.y > 0 && chunkNum != Vector2Int.up) {
            for (int i = 0; i < vWalls.GetLength (0); i++) vWalls[i, 0] = false;
        }
        if(chunkNum.y < 0 && chunkNum != Vector2Int.down) {
            for (int i = 0; i < vWalls.GetLength (0); i++) vWalls[i, mm.chunkNumCells] = false;
        }

        // Add walls around edges of the map
        if (chunkNum.x == mm.worldRadiusChunks) {
            for (int j = 0; j < hWalls.GetLength (1); j++) hWalls[mm.chunkNumCells, j] = true;
        }
        if (chunkNum.x == -mm.worldRadiusChunks) {
            for (int j = 0; j < hWalls.GetLength (1); j++) hWalls[0, j] = true;
        }
        if (chunkNum.y == mm.worldRadiusChunks) {
            for (int i = 0; i < vWalls.GetLength (0); i++) vWalls[i, mm.chunkNumCells] = true;
        }
        if (chunkNum.y == -mm.worldRadiusChunks) {
            for (int i = 0; i < vWalls.GetLength (0); i++) vWalls[i, 0] = true;
        }

        List<MeshFilter> wallCombine = new List<MeshFilter> ();
        List<MeshFilter> pillarCombine = new List<MeshFilter> ();

        // Horizontal walls
        for (int i = 0; i < hWalls.GetLength (0); i++) {
            for (int j = 0; j < hWalls.GetLength (1); j++) {
                if (!hWalls[i, j]) continue;
                GameObject g = new GameObject ();
                g.AddComponent<MeshFilter> ().mesh = mm.wallMesh;
                g.transform.position = chunkWallToWorld (i, j + 0.5f);
                g.transform.localScale = new Vector3 (mm.passagewayWidth, mm.wallHeight, mm.passagewayWidth);
                g.transform.parent = transform;
                wallCombine.Add (g.GetComponent<MeshFilter> ());
                //g.AddComponent<MeshRenderer> ().material = mm.wallMaterial;
            }
        }

        // Vertical walls
        for (int i = 0; i < vWalls.GetLength (0); i++) {
            for (int j = 0; j < vWalls.GetLength (1); j++) {
                if (!vWalls[i, j]) continue;
                GameObject g = new GameObject ();
                g.AddComponent<MeshFilter> ().mesh = mm.wallMesh;
                g.transform.position = chunkWallToWorld (i + 0.5f, j);
                g.transform.localScale = new Vector3 (mm.passagewayWidth, mm.wallHeight, mm.passagewayWidth);
                g.transform.parent = transform;
                g.transform.Rotate (Vector3.up, 90f);
                wallCombine.Add (g.GetComponent<MeshFilter> ());
                //g.AddComponent<MeshRenderer> ().material = mm.wallMaterial;
            }
        }

        // Pillars
        for(int i = 0; i <= mm.chunkNumCells; i++) {
            for(int j = 0; j <= mm.chunkNumCells; j++) {
                bool generatePillar = false;
                bool nonedge = i > 0 && j > 0 && i < mm.chunkNumCells && j < mm.chunkNumCells;
                generatePillar |= nonedge && (hWalls[i, j] != hWalls[i, j - 1] && vWalls[i, j] != vWalls[i - 1, j]); // corners
                generatePillar |= nonedge && ((hWalls[i, j] != hWalls[i, j - 1] && !vWalls[i, j] && !vWalls[i - 1, j]) || (vWalls[i, j] != vWalls[i - 1, j] && !hWalls[i, j] && !hWalls[i, j - 1])); // ends
                generatePillar |= (i > 0 && i < mm.chunkNumCells && (j == 0 || j == mm.chunkNumCells) && vWalls[i, j] != vWalls[i - 1, j]) || (j > 0 && j < mm.chunkNumCells && (i == 0 || i == mm.chunkNumCells) && hWalls[i, j] != hWalls[i, j - 1]); // edge ends
                generatePillar |= (i == 0 && (j == 0 || j == mm.chunkNumCells)) || (i == mm.chunkNumCells && (j == 0 || j == mm.chunkNumCells)); // all chunk corners
                generatePillar |= (i > 0 && i < mm.chunkNumCells && !vWalls[i, j] && !vWalls[i - 1, j]) && ((j == 0 && hWalls[i, j]) || (j == mm.chunkNumCells && hWalls[i, j - 1])); // double edge removals (case 1)
                generatePillar |= (j > 0 && j < mm.chunkNumCells && !hWalls[i, j] && !hWalls[i, j - 1]) && ((i == 0 && vWalls[i, j]) || (i == mm.chunkNumCells && vWalls[i - 1, j])); // double edge removals (case 2)
                if (generatePillar) {
                    GameObject g = new GameObject ();
                    g.AddComponent<MeshFilter> ().mesh = mm.pillarMesh;
                    g.transform.position = new Vector3 (((float) i / mm.chunkNumCells - 0.5f) * mm.chunkSize, mm.pillarHeight, ((float) j / mm.chunkNumCells - 0.5f) * mm.chunkSize) + chunkPos;
                    g.transform.parent = transform;
                    pillarCombine.Add (g.GetComponent<MeshFilter> ());
                }
            }
        }

        walls = new GameObject ();
        walls.name = "Walls " + chunkNum;
        Mesh combinedWalls = MeshCombiner.CombineMeshes (wallCombine.ToArray ());
        walls.AddComponent<MeshFilter> ().mesh = combinedWalls;
        walls.AddComponent<MeshRenderer> ().material = mm.wallMaterial;
        walls.AddComponent<MeshCollider> ().sharedMesh = combinedWalls;
        walls.layer = 7;
        walls.transform.parent = transform;

        pillars = new GameObject ();
        pillars.name = "Pillars " + chunkNum;
        Mesh combinedPillars = MeshCombiner.CombineMeshes (pillarCombine.ToArray ());
        pillars.AddComponent<MeshFilter> ().mesh = combinedPillars;
        pillars.AddComponent<MeshRenderer> ().material = mm.pillarMaterial;
        pillars.AddComponent<MeshCollider> ().sharedMesh = combinedPillars;
        pillars.layer = 7;
        pillars.transform.parent = transform;
    }

    public void CreateNavMeshFloor () {
        navFloor = Instantiate (mm.navMeshPrefab, transform);
        navFloor.name = "Floor " + chunkNum;
        navFloor.GetComponent<NavMeshSurface> ().size = new Vector3 (mm.chunkSize + 1, 5f, mm.chunkSize + 1);
        RebuildNavMesh ();
    }

    public void RebuildNavMesh() {
        navFloor.GetComponent<NavMeshSurface> ().BuildNavMesh ();
    }

    public Vector3 chunkWallToWorld (float i, float j) {
        return new Vector3 ((i / (float) mm.chunkNumCells - 0.5f) * mm.chunkSize, 0f, (j / (float) mm.chunkNumCells - 0.5f) * mm.chunkSize) + chunkPos;
    }

    public void LinkNavMeshes () {
        // +x wall
        if(chunkNum.x >= 0 || chunkNum == Vector2Int.left) {
            for(int j = 0; j < hWalls.GetLength(1); j++) {
                if (!hWalls[mm.chunkNumCells, j]) CreateOffMeshLink (chunkWallToWorld (mm.chunkNumCells, j + 0.5f), Vector3.right);
            }
        }
        // -x wall
        if(chunkNum.x <= 0 || chunkNum == Vector2Int.right) {
            for (int j = 0; j < hWalls.GetLength (1); j++) {
                if (!hWalls[0, j]) CreateOffMeshLink (chunkWallToWorld (0, j + 0.5f), Vector3.right);
            }
        }
        // +z wall
        if(chunkNum.y >= 0 || chunkNum == Vector2Int.down) {
            for (int i = 0; i < vWalls.GetLength (0); i++) {
                if (!vWalls[i, mm.chunkNumCells]) CreateOffMeshLink (chunkWallToWorld (i + 0.5f, mm.chunkNumCells), Vector3.forward);
            }
        }
        // -z wall
        if (chunkNum.y <= 0 || chunkNum == Vector2Int.up) {
            for (int i = 0; i < vWalls.GetLength (0); i++) {
                if (!vWalls[i, 0]) CreateOffMeshLink (chunkWallToWorld (i + 0.5f, 0), Vector3.forward);
            }
        }
    }

    void CreateOffMeshLink (Vector3 position, Vector3 spacing) {
        GameObject g = new GameObject ();
        g.name = "Off Mesh Link";
        g.transform.position = position;
        OffMeshLink l = g.AddComponent<OffMeshLink> ();
        Transform t1 = new GameObject ().transform;
        t1.position = position + spacing;
        t1.parent = g.transform;
        Transform t2 = new GameObject ().transform;
        t2.position = position - spacing;
        t2.parent = g.transform;
        l.startTransform = t1;
        l.endTransform = t2;
        l.biDirectional = true;
        l.UpdatePositions ();
    }

    public void SpawnResources () {
        // Ensure consistent randomness
        Random.InitState (("Resources" + mm.seed + chunkNum).GetHashCode ());

        for (int i = 0; i < mm.chunkNumCells; i++) {
            for(int j = 0; j < mm.chunkNumCells; j++) {
                for(int k = 0; k < mm.resourceChances.Length; k++) {
                    if(Random.value < mm.resourceChances[k].chance) {
                        Vector3 worldPos = chunkWallToWorld (i + 0.5f, j + 0.5f);
                        GameObject r = Instantiate (mm.resourceChances[k].prefab, transform);
                        Vector2 random = Vector3.zero;// Random.insideUnitCircle * mm.passagewayWidth / 3.0f;
                        r.transform.position = worldPos + new Vector3 (random.x, 0f, random.y);
                        break; // break inner (k) loop, don't spawn another resource on this (i, j) tile
                    }
                }
            }
        }
    }

    /*
    private void OnDrawGizmos () {
        if (!solved) return;
        Gizmos.color = Color.green;
        for (int i = 0; i < hWalls.GetLength (0); i++) {
            float x = (i / (float) mm.chunkNumCells - 0.5f) * mm.chunkSize;
            for (int j = 0; j < hWalls.GetLength (1); j++) {
                float z1 = (j / (float) mm.chunkNumCells - 0.5f) * mm.chunkSize;
                float z2 = ((j + 1) / (float) mm.chunkNumCells - 0.5f) * mm.chunkSize;
                if (hWalls[i, j]) Gizmos.DrawLine (new Vector3 (x, 0f, z1) + chunkPos, new Vector3 (x, 0f, z2) + chunkPos);
            }
        }
        for (int j = 0; j < vWalls.GetLength (1); j++) {
            float z = (j / (float) mm.chunkNumCells - 0.5f) * mm.chunkSize;
            for (int i = 0; i < vWalls.GetLength (0); i++) {
                float x1 = (i / (float) mm.chunkNumCells - 0.5f) * mm.chunkSize;
                float x2 = ((i + 1) / (float) mm.chunkNumCells - 0.5f) * mm.chunkSize;
                if (vWalls[i, j]) Gizmos.DrawLine (new Vector3 (x1, 0f, z) + chunkPos, new Vector3 (x2, 0f, z) + chunkPos);
            }
        }
    }
    */
}