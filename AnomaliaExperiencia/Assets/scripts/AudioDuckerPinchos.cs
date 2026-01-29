using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioDuckerPinchos : MonoBehaviour
{
    [Header("Trigger Audios (important)")]
    public AudioSource[] triggerAudios;

    [Header("Audios to Duck")]
    public AudioSource[] duckAudios;

    [Header("Ducking Settings")]
    [Range(0f, 1f)]
    public float duckedVolume = 0.3f;
    public float fadeTime = 0.4f;

    float[] originalVolumes;
    Coroutine duckRoutine;
    bool isDucked;

    void Awake()
    {
        originalVolumes = new float[duckAudios.Length];
        for (int i = 0; i < duckAudios.Length; i++)
            originalVolumes[i] = duckAudios[i].volume;
    }

    void Update()
    {
        bool shouldDuck = false;

        foreach (var audio in triggerAudios)
        {
            if (audio != null && audio.isPlaying)
            {
                shouldDuck = true;
                break;
            }
        }

        if (shouldDuck && !isDucked)
        {
            if (duckRoutine != null)
                StopCoroutine(duckRoutine);

            duckRoutine = StartCoroutine(FadeDuck(true));
            isDucked = true;
        }
        else if (!shouldDuck && isDucked)
        {
            if (duckRoutine != null)
                StopCoroutine(duckRoutine);

            duckRoutine = StartCoroutine(FadeDuck(false));
            isDucked = false;
        }
    }

    IEnumerator FadeDuck(bool down)
    {
        float t = 0f;

        while (t < fadeTime)
        {
            t += Time.deltaTime;
            float lerp = t / fadeTime;

            for (int i = 0; i < duckAudios.Length; i++)
            {
                float from = down ? originalVolumes[i] : duckedVolume;
                float to = down ? duckedVolume : originalVolumes[i];

                duckAudios[i].volume = Mathf.Lerp(from, to, lerp);
            }

            yield return null;
        }
    }
}