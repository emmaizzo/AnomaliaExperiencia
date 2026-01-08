using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorHandle : MonoBehaviour
{
    public float rotateAngle = -30f;
    public float rotateSpeed = 8f;

    private Quaternion closedRotation;
    private Quaternion pressedRotation;

    private bool isPressing = false;
    private bool isReturning = false;

    void Start()
    {
        closedRotation = transform.localRotation;

        pressedRotation = Quaternion.Euler(
            transform.localEulerAngles.x,
            transform.localEulerAngles.y,
            transform.localEulerAngles.z + rotateAngle
        );
    }

    void Update()
    {
        if (isPressing)
        {
            transform.localRotation = Quaternion.Slerp(
                transform.localRotation,
                pressedRotation,
                Time.deltaTime * rotateSpeed
            );

            if (Quaternion.Angle(transform.localRotation, pressedRotation) < 1f)
            {
                isPressing = false;
                isReturning = true;
            }
        }
        else if (isReturning)
        {
            transform.localRotation = Quaternion.Slerp(
                transform.localRotation,
                closedRotation,
                Time.deltaTime * rotateSpeed
            );

            if (Quaternion.Angle(transform.localRotation, closedRotation) < 1f)
            {
                isReturning = false;
            }
        }
    }

    public void PressHandle()
    {
        isPressing = true;
        isReturning = false;
    }
}