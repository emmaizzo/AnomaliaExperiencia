using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitWallTrigger : MonoBehaviour
{
    public Transform player;
    public Transform nextRoomSpawn;

    [Header("Objects to hide")]
    public GameObject[] spikesToDisable;

    Collider wallCollider;

    void Awake()
    {
        wallCollider = GetComponent<Collider>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(Transition());
        }
    }

    IEnumerator Transition()
    {
        // 1️⃣ apagar pinchos
        foreach (var s in spikesToDisable)
            s.SetActive(false);

        yield return null; // un frame (muy importante)

        // 2️⃣ mover player a la otra habitación
        CharacterController cc = player.GetComponent<CharacterController>();
        cc.enabled = false;
        player.position = nextRoomSpawn.position;
        player.rotation = nextRoomSpawn.rotation;
        cc.enabled = true;

        // 3️⃣ bloquear la pared para que no vuelva
        wallCollider.isTrigger = false;

        // 4️⃣ apagar este script (ya no se usa más)
        enabled = false;
    }
}