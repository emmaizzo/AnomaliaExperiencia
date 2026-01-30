using UnityEngine;
using System.Collections;

public class ChangeSkyboxAndFloor : MonoBehaviour
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
    public AudioClip startSceneAudio;      // al segundo de empezar
    public AudioClip afterBlackAudio;      // 👈 cuando deja de ser negro
    public AudioClip revealSound;          // refuerzo del cambio
    public AudioClip afterWhiteAudio;      // 2s después del blanco

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

        // audio inicial (opcional)
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

        // 🔁 TRANSICIÓN: NEGRO → BLANCO
        RenderSettings.skybox = skyboxWhite;
        DynamicGI.UpdateEnvironment();

        RenderSettings.ambientLight = Color.white;
        RenderSettings.ambientIntensity = 1.5f;

        floorRenderer.material = floorWhiteEmissive;

        // 🔊 AUDIO CLAVE: terminó lo negro
        if (audioSource != null && afterBlackAudio != null)
            audioSource.PlayOneShot(afterBlackAudio);

        // 🔊 sonido de revelado (opcional, puede ser el mismo)
        if (audioSource != null && revealSound != null)
            audioSource.PlayOneShot(revealSound);

        // 🔊 2 segundos después
        if (audioSource != null && afterWhiteAudio != null)
        {
            yield return new WaitForSeconds(afterWhiteDelay);
            audioSource.PlayOneShot(afterWhiteAudio);
        }
    }
}