using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CurvedGuideMissile : MonoBehaviour
{
    public Transform targetTf;
    [Space]
    [Range(-1001f, 1000f)]
    public float rotationWeight = 2;
    [Range(1f, 100f)]
    public float accelerationWeight = 10;
    [Range(5f, 100f)]
    public float maxSpeed = 30;
    [Range(1f, 100f)]
    public float minSpeed = 1;


    Transform tf;
    Vector3 currentNormDir;
    float speed;

    private void Start()
    {
        tf = transform;

        float eulerZ = tf.rotation.eulerAngles.z;
        currentNormDir = new Vector3(Mathf.Cos(eulerZ * Mathf.Deg2Rad), Mathf.Sin(eulerZ * Mathf.Deg2Rad), 0);
        speed = minSpeed;
    }

    private void Update()
    {
        Vector3 thisToTarget = targetTf.position - tf.position;
        float thisToTargetDist = thisToTarget.magnitude;


        if (thisToTargetDist != 0)
        {
            speed = accelerationWeight / (thisToTargetDist);
            speed = Mathf.Clamp(speed, minSpeed, maxSpeed);
        }

        currentNormDir = Vector3.RotateTowards(currentNormDir, thisToTarget, thisToTargetDist * thisToTargetDist * rotationWeight * Time.deltaTime, 0f).normalized;
        currentNormDir.z = 0;
        tf.position += (currentNormDir * speed * Time.deltaTime);


        float value = Vector3.Angle(currentNormDir, Vector3.right) * ((currentNormDir.y > 0) ? 1f : -1f);
        tf.rotation = Quaternion.Euler(0, 0, value);
    }
}