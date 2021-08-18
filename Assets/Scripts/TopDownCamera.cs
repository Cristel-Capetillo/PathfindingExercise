using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TopDownCamera : MonoBehaviour
{
    Transform unit;
    public float distanceFromUnit = 5f;
    public float angle = 43f;
    public float height = 9.5f;
    public float smoothLevel = 0.6f;
    Vector3 referenceToVelocity;

    void Start()
    {
        unit = GameObject.FindGameObjectWithTag("Player").transform;
        ExecuteTopDownCam();
    }

    void LateUpdate()
    {
        ExecuteTopDownCam();
    }
    
    void ExecuteTopDownCam()
    {
        if (!unit)
        {
            return; 
        }
        
        Vector3 worldPosition = (Vector3.forward * -distanceFromUnit) +(Vector3.up * height);
        Vector3 angleToViewFrom = Quaternion.AngleAxis(angle, Vector3.up) * worldPosition;
        Vector3 flatPosition = unit.position;
        flatPosition.y = 0;
        Vector3 finalPosition = flatPosition + angleToViewFrom;
        transform.position =
            Vector3.SmoothDamp(transform.position, finalPosition, ref referenceToVelocity, smoothLevel);
        transform.LookAt(flatPosition);
    }
}