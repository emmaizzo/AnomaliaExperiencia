using UnityEngine;
using System.Collections;

public class SkyboxAndFloorTimer : MonoBehaviour
{
    [Header("References")]
    public Camera mainCamera;
    public Renderer floorRenderer;
    public AudioSource audioSource;

    [Header("Skyboxes")]
    public Material skyboxBlack;
    public Material skyboxWhite;

    [Header("Floor Materials")]
    public Material floorBlack;
    public Material floorWhiteEmissive;

    [Header("Audio")]
    public AudioClip revealSound;

    [Header("Timing")]
    public float delay = 10f;

    void Start()
    {
        // ESTADO INICIAL: NEGRO
        RenderSettings.skybox = skyboxBlack;
        RenderSettings.ambientLight = Color.black;
        RenderSettings.ambientIntensity = 0f;

        mainCamera.clearFlags = CameraClearFlags.Skybox;

        floorRenderer.material = floorBlack;

        DynamicGI.UpdateEnvironment();

        StartCoroutine(ChangeToWhite());
    }

    IEnumerator ChangeToWhite()
    {
        yield return new WaitForSeconds(delay);

        // CAMBIO A BLANCO
        RenderSettings.skybox = skyboxWhite;
        DynamicGI.UpdateEnvironment();

        RenderSettings.ambientLight = Color.white;
        RenderSettings.ambientIntensity = 1.5f;

        floorRenderer.material = floorWhiteEmissive;

        // 🔊 SONIDO DE REVELACIÓN
        if (audioSource != null && revealSound != null)
        {
            audioSource.PlayOneShot(revealSound);
        }
    }
}
