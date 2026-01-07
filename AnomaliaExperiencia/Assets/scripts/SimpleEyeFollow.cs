using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EyeFollowYOnly : MonoBehaviour
{
    public Transform target;
    public float speed = 6f;
    public float maxAngle = 30f;

    float baseY;

    void Start()
    {
        baseY = transform.eulerAngles.y;
    }

    void Update()
    {
        if (!target) return;

        Vector3 dir = target.position - transform.position;
        dir.y = 0f; // 🔒 bloquea eje Y vertical

        if (dir.sqrMagnitude < 0.001f) return;

        float targetY = Quaternion.LookRotation(dir).eulerAngles.y;

        float delta = Mathf.DeltaAngle(baseY, targetY);
        float clamped = Mathf.Clamp(delta, -maxAngle, maxAngle);

        float finalY = baseY + clamped;

        float smoothY = Mathf.LerpAngle(
            transform.eulerAngles.y,
            finalY,
            Time.deltaTime * speed
        );

        transform.rotation = Quaternion.Euler(
            transform.eulerAngles.x,
            smoothY,
            transform.eulerAngles.z
        );
    }
}
