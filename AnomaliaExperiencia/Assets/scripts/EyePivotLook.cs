using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EyePivotLook : MonoBehaviour
{
    public Transform target;
    public float maxAngle = 20f;
    public float speed = 5f;

    Quaternion baseRotation;

    void Start()
    {
        baseRotation = transform.localRotation;
    }

    void LateUpdate()
    {
        if (!target) return;

        Vector3 dir = target.position - transform.position;
        Quaternion lookRot = Quaternion.LookRotation(dir, Vector3.up);

        Quaternion limited = Quaternion.RotateTowards(
            baseRotation,
            lookRot,
            maxAngle
        );

        transform.localRotation = Quaternion.Slerp(
            transform.localRotation,
            limited,
            Time.deltaTime * speed
        );
    }
}

