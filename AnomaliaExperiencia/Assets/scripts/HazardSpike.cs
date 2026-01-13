using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HazardSpike : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Algo entro al pincho: " + other.name);

        if (!other.CompareTag("Player")) return;

        Debug.Log("PLAYER TOCO PINCHOS");
        ResetManager.Instance.ResetRoom();
    }
}
