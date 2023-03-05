using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    CharacterController controller;

    public float speed = 6.0f;
    public float rotateSpeed = 1f;
    public GameObject head;
    public LayerMask groundMask;

    float startY;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        startY = transform.position.y;
    }
    
    void Start()
    {
        
    }
    
    void Update()
    {
        // Rotate the body a bit to align with the head
        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(0, head.transform.rotation.eulerAngles.y, 0), rotateSpeed*Time.deltaTime);

        // Find the position that the mouse intersects with the ground, where the ground is the groundMask
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 1000, groundMask))
        {
            // Find the hit position projected onto the XZ plane of the head
            Vector3 hitXZ = new Vector3(hit.point.x, head.transform.position.y, hit.point.z);
            // Set the head to look at the hit point
            head.transform.LookAt(hitXZ);
        }

        // Move the body in the direction of WASD controls
        Vector3 move = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        controller.Move(move * speed * Time.deltaTime);
        transform.position = new Vector3(transform.position.x, startY, transform.position.z);
    }
}
