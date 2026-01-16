using System.Collections;
using System.Collections.Generic;
using UnityEngine.Video;
using UnityEngine;

public class ProximityVideoPlayer : MonoBehaviour
{
    [Header("Video")]
    public VideoPlayer videoPlayer;

    [Header("Audio Fade")]
    public float audioFadeDuration = 1f;
    public float targetVolume = 1f;

    [Header("Light")]
    public Light targetLight;
    public float lightFadeDuration = 1f;
    public float lightIntensityOn = 1.5f;

    Coroutine audioCoroutine;
    Coroutine lightCoroutine;

    void Start()
    {
        if (videoPlayer == null)
        {
            Debug.LogError("No asignaste un VideoPlayer", this);
            return;
        }

        videoPlayer.Play();
        videoPlayer.Pause();
        videoPlayer.SetDirectAudioVolume(0, 0f);

        if (targetLight != null)
            targetLight.intensity = 0f;
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        videoPlayer.Play();
        FadeAudio(targetVolume);
        FadeLight(lightIntensityOn);
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        FadeAudio(0f);
        FadeLight(0f);
    }

    void FadeAudio(float to)
    {
        if (audioCoroutine != null)
            StopCoroutine(audioCoroutine);

        audioCoroutine = StartCoroutine(FadeAudioRoutine(to));
    }

    IEnumerator FadeAudioRoutine(float to)
    {
        float from = videoPlayer.GetDirectAudioVolume(0);
        float t = 0f;

        while (t < audioFadeDuration)
        {
            float v = Mathf.Lerp(from, to, t / audioFadeDuration);
            videoPlayer.SetDirectAudioVolume(0, v);
            t += Time.deltaTime;
            yield return null;
        }

        videoPlayer.SetDirectAudioVolume(0, to);

        if (to == 0f)
            videoPlayer.Pause();
    }

    void FadeLight(float to)
    {
        if (targetLight == null) return;

        if (lightCoroutine != null)
            StopCoroutine(lightCoroutine);

        lightCoroutine = StartCoroutine(FadeLightRoutine(to));
    }

    IEnumerator FadeLightRoutine(float to)
    {
        float from = targetLight.intensity;
        float t = 0f;

        while (t < lightFadeDuration)
        {
            targetLight.intensity = Mathf.Lerp(from, to, t / lightFadeDuration);
            t += Time.deltaTime;
            yield return null;
        }

        targetLight.intensity = to;
    }
}