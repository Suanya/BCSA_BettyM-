using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;

public class BubbleFloater : MonoBehaviour
{
    public float floatSpeed = 1f; // Speed at which the object floats
    public float windForce = 0.5f;

    private void Start()
    {
        ApplyWindForce();
    }

    void Update()
    {
        // Move the object upwards
        transform.Translate(Vector3.up * floatSpeed * Time.deltaTime);
    }

    private void ApplyWindForce()
    {
        Vector3 randomDirection = Random.insideUnitSphere;

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null ) 
        {
            rb.AddForce(randomDirection * windForce, ForceMode.Impulse);
        }
    }
}




