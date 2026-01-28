using System.Collections;
using UnityEngine;

public class CorridorIntro : MonoBehaviour
{
    [Header("Pantalla negra")]
    public CanvasGroup blackScreen;
    public float blackScreenDuration = 4f;
    public float fadeOutDuration = 2f;

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
    public AudioSource chillMusic;          // ← musicA
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

        if (voiceAudio != null)
            StartCoroutine(PlayVoiceAfterDelay(voiceAudio, voiceStartTime));

        if (secondVoiceAudio != null)
            StartCoroutine(PlaySecondVoiceWithDuck());

        StartCoroutine(IntroSequence());
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