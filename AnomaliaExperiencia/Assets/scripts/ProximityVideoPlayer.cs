using System.Collections;
using System.Collections.Generic;
using UnityEngine.Video;
using UnityEngine;

public class ProximityVideoPlayer : MonoBehaviour
{
    [Header("Video a controlar")]
    public VideoPlayer videoPlayer;

    [Header("Audio Fade")]
    public float fadeDuration = 1.0f;
    public float targetVolume = 1.0f;

    Coroutine fadeCoroutine;

    void Start()
    {
        if (videoPlayer == null)
        {
            Debug.LogError("No asignaste un VideoPlayer en el Inspector", this);
            return;
        }

        videoPlayer.Play();
        videoPlayer.Pause();

        videoPlayer.SetDirectAudioVolume(0, 0f);
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        videoPlayer.Play();
        StartFade(targetVolume);
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        StartFade(0f);
    }

    void StartFade(float toVolume)
    {
        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        fadeCoroutine = StartCoroutine(FadeAudio(toVolume));
    }

    IEnumerator FadeAudio(float to)
    {
        float from = videoPlayer.GetDirectAudioVolume(0);
        float time = 0f;

        while (time < fadeDuration)
        {
            float v = Mathf.Lerp(from, to, time / fadeDuration);
            videoPlayer.SetDirectAudioVolume(0, v);
            time += Time.deltaTime;
            yield return null;
        }

        videoPlayer.SetDirectAudioVolume(0, to);

        if (to == 0f)
            videoPlayer.Pause();
    }
}