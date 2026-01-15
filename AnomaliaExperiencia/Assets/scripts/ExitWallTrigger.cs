using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitWallTrigger : MonoBehaviour
{
    public Transform player;
    public Transform nextRoomSpawn;

    [Header("Objects to hide")]
    public GameObject[] spikesToDisable;

    [Header("Skybox")]
    public Material newSkybox;

    [Header("Flashlight")]
    public Light flashlight;
    public float flashlightFadeDuration = 0.5f;

    [Header("Room Lights")]
    public Light[] roomLights;   // las 2 luces
    public float roomLightIntensity = 1f;

    Collider wallCollider;
    bool used = false;

    void Awake()
    {
        wallCollider = GetComponent<Collider>();

        // Asegurarse de que las luces estén apagadas al inicio
        foreach (var l in roomLights)
        {
            if (l != null)
                l.enabled = false;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (!used && other.CompareTag("Player"))
        {
            used = true;
            StartCoroutine(Transition());
        }
    }

    IEnumerator Transition()
    {
        // 1️⃣ apagar pinchos
        foreach (var s in spikesToDisable)
            s.SetActive(false);

        // 2️⃣ apagar linterna suavemente
        if (flashlight != null)
            yield return StartCoroutine(FadeFlashlight());

        // 3️⃣ cambiar skybox
        if (newSkybox != null)
        {
            RenderSettings.skybox = newSkybox;
            DynamicGI.UpdateEnvironment();
        }

        yield return null;

        // 4️⃣ mover player
        CharacterController cc = player.GetComponent<CharacterController>();
        cc.enabled = false;
        player.position = nextRoomSpawn.position;
        player.rotation = nextRoomSpawn.rotation;
        cc.enabled = true;

        // 5️⃣ prender luces de la habitación
        foreach (var l in roomLights)
        {
            if (l != null)
            {
                l.enabled = true;
                l.intensity = roomLightIntensity;
            }
        }

        // 6️⃣ bloquear pared
        wallCollider.isTrigger = false;

        enabled = false;
    }

    IEnumerator FadeFlashlight()
    {
        float startIntensity = flashlight.intensity;
        float t = 0f;

        while (t < flashlightFadeDuration)
        {
            t += Time.deltaTime;
            flashlight.intensity = Mathf.Lerp(startIntensity, 0f, t / flashlightFadeDuration);
            yield return null;
        }

        flashlight.intensity = 0f;
    }
}