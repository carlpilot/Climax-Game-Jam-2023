using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupOnHit : MonoBehaviour
{
    public Item item;
    public Transform rotater;
    public float rotationSpeed = 100f;
    public float bounceSpeed = 1f;
    public float bounceHeight = 1f;
    public float heightOffset = 1;
    
    void Start()
    {
        var mesh = Instantiate(item.pickupPrefab, rotater);
    }
    
    void Update()
    {
        rotater.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
        // Move the rotater up and down with a sine wave
        rotater.localPosition = new Vector3(0, Mathf.Sin(Time.time*bounceSpeed) *bounceHeight+heightOffset, 0);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.transform.parent && other.transform.parent.gameObject.CompareTag("Player"))
        {
            if (other.GetComponent<PlayerInventory>().AddItem(item))
            {
                Destroy(gameObject);
            }
        }
    }
}
