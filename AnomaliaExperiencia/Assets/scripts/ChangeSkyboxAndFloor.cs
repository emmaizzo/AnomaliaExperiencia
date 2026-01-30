using UnityEngine;
using System.Collections;

public class ChangeSkyboxAndFloor : MonoBehaviour
{
    [Header("References")]
    public Camera mainCamera;
    public Renderer floorRenderer;

    [Header("Audio Sources")]
    public AudioSource audioSource;        // 👈 el de siempre
    public AudioSource afterBlackSource;   // 👈 NUEVO: solo para after black

    [Header("Skyboxes")]
    public Material skyboxBlack;
    public Material skyboxWhite;

    [Header("Floor Materials")]
    public Material floorBlack;
    public Material floorWhiteEmissive;

    [Header("Audio Clips")]
    public AudioClip startSceneAudio;
    public AudioClip afterBlackAudio;      // ahora va en afterBlackSource
    public AudioClip revealSound;
    public AudioClip afterWhiteAudio;

    [Header("Timing")]
    public float startAudioDelay = 1f;
    public float delay = 10f;
    public float afterWhiteDelay = 2f;

    void Start()
    {
        // ESTADO INICIAL: NEGRO
        RenderSettings.skybox = skyboxBlack;
        RenderSettings.ambientLight = Color.black;
        RenderSettings.ambientIntensity = 0f;

        mainCamera.clearFlags = CameraClearFlags.Skybox;
        floorRenderer.material = floorBlack;

        DynamicGI.UpdateEnvironment();

        // audio inicial
        if (startSceneAudio != null && audioSource != null)
            StartCoroutine(PlayStartAudio());

        StartCoroutine(ChangeToWhite());
    }

    IEnumerator PlayStartAudio()
    {
        yield return new WaitForSeconds(startAudioDelay);
        audioSource.PlayOneShot(startSceneAudio);
    }

    IEnumerator ChangeToWhite()
    {
        yield return new WaitForSeconds(delay);

        // 🔁 NEGRO → BLANCO
        RenderSettings.skybox = skyboxWhite;
        DynamicGI.UpdateEnvironment();

        RenderSettings.ambientLight = Color.white;
        RenderSettings.ambientIntensity = 1.5f;

        floorRenderer.material = floorWhiteEmissive;

        // 🔊 AFTER BLACK → AudioSource dedicado
        if (afterBlackSource != null && afterBlackAudio != null)
        {
            afterBlackSource.clip = afterBlackAudio;
            afterBlackSource.Play();
        }

        // 🔊 reveal sound (queda igual)
        if (audioSource != null && revealSound != null)
            audioSource.PlayOneShot(revealSound);

        // 🔊 after white (queda igual)
        if (audioSource != null && afterWhiteAudio != null)
        {
            yield return new WaitForSeconds(afterWhiteDelay);
            audioSource.PlayOneShot(afterWhiteAudio);
        }
    }
}