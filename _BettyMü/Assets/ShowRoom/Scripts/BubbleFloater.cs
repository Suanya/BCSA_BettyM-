using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BubbleFloater : MonoBehaviour
{
    public float floatSpeed = 1f; // Speed at which the object floats

    void Update()
    {
        // Move the object upwards
        transform.Translate(Vector3.up * floatSpeed * Time.deltaTime);
    }
}




