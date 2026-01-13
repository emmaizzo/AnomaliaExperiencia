using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapdoorTrigger : MonoBehaviour
{
    bool playerInside;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = true;
            Debug.Log("Player dentro de trampilla");
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = false;
        }
    }

    void Update()
    {
        if (playerInside && Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("E presionada en trampilla");
            StressManager.Instance.IncreaseStress();
            Debug.Log("SpeedMultiplier = " + StressManager.Instance.speedMultiplier);
        }
    }
}