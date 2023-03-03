using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject player;
    private Vector3 offset;
    public float speed = 1.0f;
    public float rotationSpeed = 1.0f;

    void Awake()
    {
        
    }
    
    void Start()
    {
        // Set the offset to our current offset
        offset = transform.position - player.transform.position;
    }
    
    void Update()
    {
        // Move the camera to the players position + the offset with a lerp
        transform.position = Vector3.Lerp(transform.position, player.transform.position + offset, speed * Time.deltaTime);
        // Look at the player with a lerp
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(player.transform.position - transform.position), rotationSpeed * Time.deltaTime);
    }
}
