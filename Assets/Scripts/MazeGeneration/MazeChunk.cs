using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeChunk : MonoBehaviour
{
    public MazeMaker mm;
    public Vector2Int chunkNum;

    public void Generate () {
        CreateFloor ();
    }

    public void CreateFloor () {
        GameObject g = new GameObject ("Floor Mesh " + chunkNum);
        g.transform.parent = transform;
        g.transform.localPosition = Vector3.zero;
        MeshFilter mf = g.AddComponent<MeshFilter> ();
        mf.mesh = mm.GetFloorMesh ();
        MeshRenderer mr = g.AddComponent<MeshRenderer> ();
        mr.material = mm.floorMaterial;
    }
}
