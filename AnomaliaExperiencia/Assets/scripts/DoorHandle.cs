using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorHandle : MonoBehaviour
{
    public float pressAngle = -30f;
    public float speed = 10f;
    public float holdTime = 0.25f;

    [Header("Axis")]
    public Vector3 localAxis = Vector3.forward; // ← editable

    private Quaternion closedRotation;
    private Quaternion pressedRotation;

    private Coroutine animRoutine;

    void Start()
    {
        closedRotation = transform.localRotation;
        pressedRotation = closedRotation * Quaternion.AngleAxis(pressAngle, localAxis);
    }

    public void PressHandle()
    {
        if (animRoutine != null)
            StopCoroutine(animRoutine);

        animRoutine = StartCoroutine(HandleRoutine());
    }

    private IEnumerator HandleRoutine()
    {
        // bajar
        while (Quaternion.Angle(transform.localRotation, pressedRotation) > 0.5f)
        {
            transform.localRotation = Quaternion.Slerp(
                transform.localRotation,
                pressedRotation,
                Time.deltaTime * speed
            );
            yield return null;
        }

        yield return new WaitForSeconds(holdTime);

        // subir
        while (Quaternion.Angle(transform.localRotation, closedRotation) > 0.5f)
        {
            transform.localRotation = Quaternion.Slerp(
                transform.localRotation,
                closedRotation,
                Time.deltaTime * speed
            );
            yield return null;
        }
    }
}