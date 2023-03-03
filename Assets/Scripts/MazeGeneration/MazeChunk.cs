using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeChunk : MonoBehaviour {

    public MazeMaker mm;
    public Vector2Int chunkNum;

    Vector2Int[,] parents;
    bool[,] hWalls;
    bool[,] vWalls;

    Vector3 chunkPos;
    Vector2Int unassigned = new Vector2Int (-1, -1);
    bool solved = false;

    public void Generate () {
        chunkPos = new Vector3 (chunkNum.x, 0f, chunkNum.y) * mm.chunkSize;
        CreateFloor ();
        if (chunkNum != Vector2.zero) {
            SolveMaze ();
            SpawnWallObjects ();
        }
    }

    void CreateFloor () {
        GameObject g = new GameObject ("Floor Mesh " + chunkNum);
        g.transform.parent = transform;
        g.transform.localPosition = Vector3.zero;
        MeshFilter mf = g.AddComponent<MeshFilter> ();
        mf.mesh = mm.GetFloorMesh ();
        MeshRenderer mr = g.AddComponent<MeshRenderer> ();
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

    void SpawnWallObjects() {
        for (int i = 0; i < hWalls.GetLength (0); i++) {
            for (int j = 0; j < hWalls.GetLength (1); j++) {
                if (!hWalls[i, j]) continue;
                GameObject g = GameObject.CreatePrimitive (PrimitiveType.Cube);
                g.transform.position = new Vector3 ((i / (float) mm.chunkNumCells - 0.5f) * mm.chunkSize, 1f, ((j + 0.5f) / (float) mm.chunkNumCells - 0.5f) * mm.chunkSize) + chunkPos;
                g.transform.localScale = new Vector3 (0.5f, 2f, mm.chunkSize / mm.chunkNumCells);
                g.transform.parent = transform;
            }
        }
        for (int i = 0; i < vWalls.GetLength (0); i++) {
            for (int j = 0; j < vWalls.GetLength (1); j++) {
                if (!vWalls[i, j]) continue;
                GameObject g = GameObject.CreatePrimitive (PrimitiveType.Cube);
                g.transform.position = new Vector3 (((i + 0.5f) / (float) mm.chunkNumCells - 0.5f) * mm.chunkSize, 1f, (j / (float) mm.chunkNumCells - 0.5f) * mm.chunkSize) + chunkPos;
                g.transform.localScale = new Vector3 (mm.chunkSize / mm.chunkNumCells, 2f, 0.5f);
                g.transform.parent = transform;
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