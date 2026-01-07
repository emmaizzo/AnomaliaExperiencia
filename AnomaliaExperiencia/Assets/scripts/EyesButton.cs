using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EyesButton : MonoBehaviour
{
    public EyesRoomManager manager;
    bool playerInside;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            playerInside = true;
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            playerInside = false;
    }

    void Update()
    {
        if (playerInside && Input.GetKeyDown(KeyCode.E))
            manager.SpawnMoreEyes();
    }
}


