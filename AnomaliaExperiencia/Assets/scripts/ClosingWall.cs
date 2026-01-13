using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClosingWall : MonoBehaviour
{
    [Header("Movement")]
    public Vector3 moveDirection = Vector3.forward;
    public float baseSpeed = 0.5f;

    void Update()
    {
        // multiplicador global de stress
        float stressMultiplier = 1f;

        if (StressManager.Instance != null)
            stressMultiplier = StressManager.Instance.speedMultiplier;

        transform.Translate(moveDirection * baseSpeed * stressMultiplier * Time.deltaTime, Space.World);
    }
}