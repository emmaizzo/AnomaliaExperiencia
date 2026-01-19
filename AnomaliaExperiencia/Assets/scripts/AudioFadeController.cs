using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioFadeController : MonoBehaviour
{
    public float fadeDuration = 2f;

    AudioSource[] allSources;

    void Awake()
    {
        allSources = FindObjectsOfType<AudioSource>();
    }

    public void FadeOutAll()
    {
        StartCoroutine(FadeOutCoroutine());
    }

    IEnumerator FadeOutCoroutine()
    {
        float t = 0f;

        float[] startVolumes = new float[allSources.Length];
        for (int i = 0; i < allSources.Length; i++)
            startVolumes[i] = allSources[i].volume;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float k = 1f - (t / fadeDuration);

            for (int i = 0; i < allSources.Length; i++)
            {
                if (allSources[i] != null)
                    allSources[i].volume = startVolumes[i] * k;
            }

            yield return null;
        }

        for (int i = 0; i < allSources.Length; i++)
        {
            if (allSources[i] != null)
                allSources[i].Stop();
        }
    }
}