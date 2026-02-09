using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioDuckerAgua : MonoBehaviour
{
    [Header("Audios to Duck")]
    public AudioSource[] duckAudios;

    [Header("Ducking Settings")]
    [Range(0f, 1f)]
    public float duckMultiplier = 0.3f;
    public float fadeTime = 0.4f;

    float[] originalVolumes;
    Coroutine duckRoutine;
    Coroutine forcedDuckRoutine;
    bool isDucked;

    void Awake()
    {
        originalVolumes = new float[duckAudios.Length];
        for (int i = 0; i < duckAudios.Length; i++)
            originalVolumes[i] = duckAudios[i].volume;
    }

    // 🔥 LLAMAR DESDE OTROS SCRIPTS
    public void ForceDuck(float duration)
    {
        if (forcedDuckRoutine != null)
            StopCoroutine(forcedDuckRoutine);

        forcedDuckRoutine = StartCoroutine(ForceDuckRoutine(duration));
    }

    IEnumerator ForceDuckRoutine(float duration)
    {
        // bajar
        StartDuck();

        yield return new WaitForSeconds(duration);

        // subir
        StopDuck();
    }

    void StartDuck()
    {
        if (isDucked) return;

        if (duckRoutine != null)
            StopCoroutine(duckRoutine);

        duckRoutine = StartCoroutine(FadeDuck(true));
        isDucked = true;
    }

    void StopDuck()
    {
        if (!isDucked) return;

        if (duckRoutine != null)
            StopCoroutine(duckRoutine);

        duckRoutine = StartCoroutine(FadeDuck(false));
        isDucked = false;
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
                float from = duckAudios[i].volume;
                float to = down
                    ? originalVolumes[i] * duckMultiplier
                    : originalVolumes[i];

                duckAudios[i].volume = Mathf.Lerp(from, to, lerp);
            }

            yield return null;
        }
    }
}