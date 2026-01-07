using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EyeLocalLook : MonoBehaviour
{
    public Transform target;
    public float maxAngle = 20f;
    public float speed = 4f;

    Quaternion baseRotation;

    void Start()
    {
        baseRotation = transform.localRotation;
    }

    void LateUpdate()
    {
        if (!target) return;

        Vector3 dir = target.position - transform.position;
        Quaternion lookRot = Quaternion.LookRotation(dir);

        Quaternion limited = Quaternion.RotateTowards(
            baseRotation,
            lookRot,
            maxAngle
        );

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            limited,
            Time.deltaTime * speed
        );
    }
}

