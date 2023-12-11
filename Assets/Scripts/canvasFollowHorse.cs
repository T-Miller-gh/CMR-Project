using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class canvasFollowHorse : MonoBehaviour
{
    public Transform horseTarget;
    public float heightOffset = 1.5f; 

    // Update is called once per frame
    void FixedUpdate()
    {
        // make the canvas follow the horse, and add some height so it floats above horse
        Vector3 targetPosition = horseTarget.position + Vector3.up * heightOffset;
        transform.position = targetPosition; 
    }
}
