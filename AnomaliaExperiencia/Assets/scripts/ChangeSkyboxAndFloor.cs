using UnityEngine;
using System.Collections;

public class ChangeSkyboxAndFloor : MonoBehaviour
{
    [Header("References")]
    public Camera mainCamera;
    public Renderer floorRenderer;

    [Header("Audio Sources")]
    public AudioSource audioSource;
    public AudioSource afterBlackSource;

    [Header("Skyboxes")]
    public Material skyboxBlack;
    public Material skyboxWhite;

    [Header("Floor Materials")]
    public Material floorBlack;
    public Material floorWhiteEmissive;

    [Header("Audio Clips")]
    public AudioClip startSceneAudio;
    public AudioClip afterBlackAudio;
    public AudioClip revealSound;
    public AudioClip afterWhiteAudio;

    [Header("Timing")]
    public float startAudioDelay = 1f;
    public float delay = 10f;
    public float afterWhiteDelay = 2f;

    [Header("Title")]
    public CanvasGroup titleCanvas;
    public float titleFadeDuration = 0.5f; // 👈 fade suave

    void Start()
    {
        // ESTADO INICIAL: NEGRO
        RenderSettings.skybox = skyboxBlack;
        RenderSettings.ambientLight = Color.black;
        RenderSettings.ambientIntensity = 0f;

        mainCamera.clearFlags = CameraClearFlags.Skybox;
        floorRenderer.material = floorBlack;

        DynamicGI.UpdateEnvironment();

        if (startSceneAudio != null && audioSource != null)
            StartCoroutine(PlayStartAudio());

        StartCoroutine(HandleTitle());
        StartCoroutine(ChangeToWhite());
    }

    IEnumerator PlayStartAudio()
    {
        yield return new WaitForSeconds(startAudioDelay);
        audioSource.PlayOneShot(startSceneAudio);
    }

    IEnumerator HandleTitle()
    {
        if (titleCanvas == null)
            yield break;

        // oculto al inicio
        titleCanvas.alpha = 0f;

        // aparece 1 segundo después del inicio
        yield return new WaitForSeconds(1f);
        yield return StartCoroutine(FadeCanvas(titleCanvas, 0f, 1f, titleFadeDuration));

        // permanece visible hasta 1 segundo antes del cambio
        float visibleTime = delay - 2f - titleFadeDuration;
        yield return new WaitForSeconds(Mathf.Max(0f, visibleTime));

        // fade out
        yield return StartCoroutine(FadeCanvas(titleCanvas, 1f, 0f, titleFadeDuration));
    }

    IEnumerator FadeCanvas(CanvasGroup canvas, float from, float to, float duration)
    {
        float t = 0f;
        canvas.alpha = from;

        while (t < duration)
        {
            t += Time.deltaTime;
            canvas.alpha = Mathf.Lerp(from, to, t / duration);
            yield return null;
        }

        canvas.alpha = to;
    }

    IEnumerator ChangeToWhite()
    {
        yield return new WaitForSeconds(delay);

        // NEGRO → BLANCO
        RenderSettings.skybox = skyboxWhite;
        DynamicGI.UpdateEnvironment();

        RenderSettings.ambientLight = Color.white;
        RenderSettings.ambientIntensity = 1.5f;

        floorRenderer.material = floorWhiteEmissive;

        if (afterBlackSource != null && afterBlackAudio != null)
        {
            afterBlackSource.clip = afterBlackAudio;
            afterBlackSource.Play();
        }

        if (audioSource != null && revealSound != null)
            audioSource.PlayOneShot(revealSound);

        if (audioSource != null && afterWhiteAudio != null)
        {
            yield return new WaitForSeconds(afterWhiteDelay);
            audioSource.PlayOneShot(afterWhiteAudio);
        }
    }
}