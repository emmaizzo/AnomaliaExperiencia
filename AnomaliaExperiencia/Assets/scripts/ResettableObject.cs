using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResettableObject : MonoBehaviour
{
    Vector3 startPos;
    Quaternion startRot;
    Vector3 startScale;

    MonoBehaviour[] scripts;

    void Awake()
    {
        startPos = transform.position;
        startRot = transform.rotation;
        startScale = transform.localScale;

        scripts = GetComponents<MonoBehaviour>();
    }

    public void ResetState()
    {
        transform.position = startPos;
        transform.rotation = startRot;
        transform.localScale = startScale;

        foreach (var s in scripts)
        {
            if (s != this)
                s.enabled = true;
        }
    }
}