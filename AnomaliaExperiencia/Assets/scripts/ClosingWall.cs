using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClosingWall : MonoBehaviour
{
    public enum MoveDirection { Forward, Backward }

    [Header("Movement")]
    public MoveDirection direction;
    public float directionMultiplier = 1f;
    public float maxDistance = 3f;

    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        if (StressManager.Instance == null) return;

        float speed = StressManager.Instance.GetWallSpeed() * directionMultiplier;
        float move = speed * Time.deltaTime;

        if (direction == MoveDirection.Forward)
            transform.Translate(0f, 0f, move);
        else
            transform.Translate(0f, 0f, -move);

        float dist = Vector3.Distance(startPos, transform.position);
        if (dist >= maxDistance)
            enabled = false;
    }
}