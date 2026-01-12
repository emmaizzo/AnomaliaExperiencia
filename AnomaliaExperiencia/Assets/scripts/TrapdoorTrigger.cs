using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapdoorTrigger : MonoBehaviour
{
    bool used = false;

    void OnTriggerEnter(Collider other)
    {
        if (used) return;
        if (!other.CompareTag("Player")) return;

        used = true;
        StressManager.Instance.IncreaseStress();

        Debug.Log("Stress increased");
    }
}