using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EyeLookAtPlayer : MonoBehaviour
{
    Transform target;
    Quaternion baseRotation;

    public float maxAngle = 25f;

    void Start()
    {
        baseRotation = transform.rotation;
    }

    public void SetTarget(Transform t)
    {
        target = t;
    }

    void LateUpdate()
    {
        if (!target) return;

        Vector3 dir = target.position - transform.position;
        Quaternion lookRot = Quaternion.LookRotation(dir);

        Quaternion delta = Quaternion.Inverse(baseRotation) * lookRot;
        delta = Quaternion.RotateTowards(
            Quaternion.identity,
            delta,
            maxAngle
        );

        transform.rotation = baseRotation * delta;
    }
}