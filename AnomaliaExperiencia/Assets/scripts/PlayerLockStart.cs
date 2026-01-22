using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLockStart : MonoBehaviour
{
    public GameObject playerCapsule;
    public float tiempoBloqueado = 10f;

    void Start()
    {
        if (playerCapsule != null)
            playerCapsule.SetActive(false);

        StartCoroutine(ReactivarJugador());
    }

    IEnumerator ReactivarJugador()
    {
        yield return new WaitForSeconds(tiempoBloqueado);

        if (playerCapsule != null)
            playerCapsule.SetActive(true);
    }
}