using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject player;
    private Vector3 offset;
    public float speed = 1.0f;
    public float rotationSpeed = 1.0f;

    public static float cameraShake = 0;

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
        // Calcualte a random quaternion to add to the look rotation
        var randomRotation = Quaternion.Euler(Random.Range(-cameraShake, cameraShake), Random.Range(-cameraShake, cameraShake), Random.Range(-cameraShake, cameraShake));
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(player.transform.position - transform.position), rotationSpeed * Time.deltaTime);
        // Add the random rotation to the look rotation
        transform.rotation = transform.rotation * randomRotation;
    }
}
