using System.Collections;
using UnityEngine;

public class CorridorIntro : MonoBehaviour
{
    [Header("Pantalla negra")]
    public CanvasGroup blackScreen;
    public float blackScreenDuration = 4f;
    public float fadeOutDuration = 2f;

    [Header("Imagen / Título")]
    public CanvasGroup titleImage;
    public float titleFadeDuration = 0.5f;

    [Header("Player")]
    public MonoBehaviour playerMovementScript;

    [Header("Audio del pasillo")]
    public CorridorAudioByDistance corridorAudio;

    [Header("Audio de voz (intro)")]
    public AudioSource voiceAudio;
    public float voiceStartTime = 0f;

    [Header("Audio extra (consigna)")]
    public AudioSource secondVoiceAudio;
    public float secondVoiceTime = 5f;

    [Header("Chill music ducking")]
    public AudioSource chillMusic;
    [Range(0f, 1f)] public float duckAmount = 0.6f;
    public float duckDuration = 3f;

    float originalChillVolume;

    void Start()
    {
        Time.timeScale = 0f;

        if (playerMovementScript != null)
            playerMovementScript.enabled = false;

        if (corridorAudio != null)
            corridorAudio.enabled = false;

        if (chillMusic != null)
            originalChillVolume = chillMusic.volume;

        blackScreen.alpha = 1f;
        blackScreen.blocksRaycasts = true;

        if (titleImage != null)
            titleImage.alpha = 0f;

        if (voiceAudio != null)
            StartCoroutine(PlayVoiceAfterDelay(voiceAudio, voiceStartTime));

        if (secondVoiceAudio != null)
            StartCoroutine(PlaySecondVoiceWithDuck());

        StartCoroutine(IntroSequence());
        StartCoroutine(TitleSequence());
    }

    IEnumerator IntroSequence()
    {
        yield return new WaitForSecondsRealtime(blackScreenDuration);

        float t = 0f;
        while (t < fadeOutDuration)
        {
            t += Time.unscaledDeltaTime;
            blackScreen.alpha = Mathf.Lerp(1f, 0f, t / fadeOutDuration);
            yield return null;
        }

        blackScreen.alpha = 0f;
        blackScreen.blocksRaycasts = false;

        Time.timeScale = 1f;

        if (playerMovementScript != null)
            playerMovementScript.enabled = true;

        if (corridorAudio != null)
            corridorAudio.enabled = true;
    }

    IEnumerator TitleSequence()
    {
        // ⏱️ aparece 1 segundo después de empezar el black screen
        yield return new WaitForSecondsRealtime(1f);

        // Fade IN
        float t = 0f;
        while (t < titleFadeDuration)
        {
            t += Time.unscaledDeltaTime;
            titleImage.alpha = Mathf.Lerp(0f, 1f, t / titleFadeDuration);
            yield return null;
        }

        titleImage.alpha = 1f;

        // ⏱️ se queda hasta 1 segundo antes de que termine el black screen
        float visibleTime = blackScreenDuration - 3f;
        yield return new WaitForSecondsRealtime(visibleTime);

        // Fade OUT
        t = 0f;
        while (t < titleFadeDuration)
        {
            t += Time.unscaledDeltaTime;
            titleImage.alpha = Mathf.Lerp(1f, 0f, t / titleFadeDuration);
            yield return null;
        }

        titleImage.alpha = 0f;
    }

    IEnumerator PlayVoiceAfterDelay(AudioSource audio, float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        audio.Play();
    }

    // 🎙️ Consigna + baja chill music
    IEnumerator PlaySecondVoiceWithDuck()
    {
        yield return new WaitForSecondsRealtime(secondVoiceTime);

        if (chillMusic != null)
            chillMusic.volume = originalChillVolume * duckAmount;

        secondVoiceAudio.Play();

        yield return new WaitForSecondsRealtime(duckDuration);

        if (chillMusic != null)
            chillMusic.volume = originalChillVolume;
    }
}