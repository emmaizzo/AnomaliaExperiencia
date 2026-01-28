using System.Collections;
using UnityEngine;

public class AudioFadeController : MonoBehaviour
{
    public float fadeDuration = 2f;

    [Header("Audios a IGNORAR en el fade")]
    public AudioSource[] ignoreSources;

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
        {
            if (allSources[i] != null)
                startVolumes[i] = allSources[i].volume;
        }

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float k = 1f - (t / fadeDuration);

            for (int i = 0; i < allSources.Length; i++)
            {
                AudioSource src = allSources[i];
                if (src == null) continue;
                if (IsIgnored(src)) continue;

                src.volume = startVolumes[i] * k;
            }

            yield return null;
        }

        // Stop SOLO los que no fueron ignorados
        for (int i = 0; i < allSources.Length; i++)
        {
            AudioSource src = allSources[i];
            if (src == null) continue;
            if (IsIgnored(src)) continue;

            src.Stop();
        }
    }

    bool IsIgnored(AudioSource source)
    {
        if (ignoreSources == null) return false;

        foreach (var ignored in ignoreSources)
        {
            if (ignored == source)
                return true;
        }

        return false;
    }
}