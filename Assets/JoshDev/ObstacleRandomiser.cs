using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.AI.Navigation;

public class ObstacleRandomiser : MonoBehaviour
{
    public float range = 10f;
    public NavMeshSurface surface;
    void Awake()
    {
        
    }
    
    void Start()
    {
        // For every child of this object, move it to a random position
        foreach (Transform child in transform)
        {
            child.position = new Vector3(Random.Range(-range, range), child.position.y, Random.Range(-range, range));
        }
        surface.BuildNavMesh();
    }
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            // For every child of this object, move it to a random position
            foreach (Transform child in transform)
            {
                child.position = new Vector3(Random.Range(-range, range), child.position.y, Random.Range(-range, range));
            }
            surface.BuildNavMesh();
        }
    }
}
