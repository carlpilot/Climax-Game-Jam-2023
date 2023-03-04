using UnityEngine;

public static class MeshCombiner {

    public static Mesh CombineMeshes (MeshFilter[] meshFilters) {
        // Create a new empty mesh
        Mesh combinedMesh = new Mesh ();

        // Combine all meshes into the new mesh
        CombineInstance[] combineInstances = new CombineInstance[meshFilters.Length];
        for (int i = 0; i < meshFilters.Length; i++) {
            CombineInstance c = new CombineInstance ();
            c.mesh = meshFilters[i].sharedMesh;
            c.transform = meshFilters[i].transform.localToWorldMatrix;
            combineInstances[i] = c;
            Object.Destroy (meshFilters[i].gameObject);
        }
        combinedMesh.CombineMeshes (combineInstances);

        // Recalculate the mesh normals and bounds
        combinedMesh.RecalculateNormals ();
        combinedMesh.RecalculateBounds ();

        return combinedMesh;
    }
}