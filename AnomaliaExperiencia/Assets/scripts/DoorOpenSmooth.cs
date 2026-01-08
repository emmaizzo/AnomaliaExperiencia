using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorOpenSmooth : MonoBehaviour
{
    public float openAngle = 90f;
    public float speed = 2f;

    [Header("Optional")]
    public DoorHandle handle;

    private Quaternion closedRotation;
    private Quaternion openRotation;

    private bool isOpen = false;
    private bool isLocked = false;

    void Start()
    {
        closedRotation = transform.rotation;

        openRotation = Quaternion.Euler(
            transform.eulerAngles.x,
            transform.eulerAngles.y + openAngle,
            transform.eulerAngles.z
        );
    }

    void Update()
    {
        Quaternion target = isOpen ? openRotation : closedRotation;

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            target,
            Time.deltaTime * speed
        );
    }

    public void OpenDoor()
    {
        if (isLocked) return;

        isOpen = true;

        if (handle != null)
            handle.PressHandle();
    }

    public void CloseAndLockDoor()
    {
        isOpen = false;
        isLocked = true;
    }
}
