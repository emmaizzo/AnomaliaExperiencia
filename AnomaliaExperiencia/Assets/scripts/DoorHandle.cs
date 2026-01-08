using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorHandle : MonoBehaviour
{
    public float pressAngle = -30f;
    public float speed = 6f;

    private Quaternion closedRotation;
    private Quaternion pressedRotation;

    private bool isPressed = false;

    void Start()
    {
        closedRotation = transform.localRotation;

        pressedRotation = Quaternion.Euler(
            transform.localEulerAngles.x,
            transform.localEulerAngles.y,
            transform.localEulerAngles.z + pressAngle
        );
    }

    void Update()
    {
        Quaternion target = isPressed ? pressedRotation : closedRotation;

        transform.localRotation = Quaternion.Slerp(
            transform.localRotation,
            target,
            Time.deltaTime * speed
        );
    }

    public void PressHandle()
    {
        isPressed = true;
    }

    public void CloseHandle()
    {
        isPressed = false;
    }
}
