using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;

public class BubbleFloater : MonoBehaviour
{
    public float floatSpeed = 1f; // Speed at which the object floats
    public float windStrength = 0.1f; // Strength of the wind
    public float windSmoothing = 2f; // Smoothing factor for wind direction changes

    
    private Vector3 windForce; // Current wind force applied to the object

    void Start()
    {
        // Initialize windForce
        windForce = CalculateWindForce();
    }

    void Update()
    {
        // Move the object upwards
        transform.Translate(Vector3.up * floatSpeed * Time.deltaTime);

        // Update wind force
        windForce = Vector3.Lerp(windForce, CalculateWindForce(), Time.deltaTime * windSmoothing);

        // Apply wind force to the object
        transform.position += windForce * Time.deltaTime;
    }

    Vector3 CalculateWindForce()
    {
        // Calculate wind direction using Perlin noise
        float windOffsetX = Mathf.PerlinNoise(Time.time, 0) * 2 - 1; // Random value between -1 and 1
        float windOffsetZ = Mathf.PerlinNoise(0, Time.time) * 2 - 1; // Random value between -1 and 1

        Vector3 windDirection = new Vector3(windOffsetX, 0, windOffsetZ).normalized;
        return windDirection * windStrength;
    }
}





