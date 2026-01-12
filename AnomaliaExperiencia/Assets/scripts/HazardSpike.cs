using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HazardSpike : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        StressManager.Instance.ResetScene();
    }
}