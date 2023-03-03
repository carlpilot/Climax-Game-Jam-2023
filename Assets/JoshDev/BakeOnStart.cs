using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.AI.Navigation;

public class BakeOnStart : MonoBehaviour
{

    void Awake()
    {
        
    }
    
    void Start()
    {
        GetComponent<NavMeshSurface>().BuildNavMesh();
    }
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            GetComponent<NavMeshSurface>().BuildNavMesh();
        }
    }
}
