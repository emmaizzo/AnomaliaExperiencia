using System.Collections;
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
    bool forceDuckActive;

    void Awake()
    {
        originalVolumes = new float[duckAudios.Length];

        for (int i = 0; i < duckAudios.Length; i++)
        {
            if (duckAudios[i] != null)
                originalVolumes[i] = duckAudios[i].volume;
        }
    }

    void Update()
    {
        if (forceDuckActive)
            return;

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
            StartDuck(true);
        }
        else if (!shouldDuck && isDucked)
        {
            StartDuck(false);
        }
    }

    void StartDuck(bool down)
    {
        if (duckRoutine != null)
            StopCoroutine(duckRoutine);

        duckRoutine = StartCoroutine(FadeDuck(down));
        isDucked = down;
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
                if (duckAudios[i] == null) continue;

                float from = down ? originalVolumes[i] : duckedVolume;
                float to = down ? duckedVolume : originalVolumes[i];

                duckAudios[i].volume = Mathf.Lerp(from, to, lerp);
            }

            yield return null;
        }
    }

    // ============================
    // 🔥 DUCK FORZADO (CLAVE)
    // ============================

    public void ForceDuck(float duration)
    {
        if (duckRoutine != null)
            StopCoroutine(duckRoutine);

        StartCoroutine(ForceDuckRoutine(duration));
    }

    IEnumerator ForceDuckRoutine(float duration)
    {
        forceDuckActive = true;

        yield return StartCoroutine(FadeDuck(true));
        yield return new WaitForSeconds(duration);
        yield return StartCoroutine(FadeDuck(false));

        forceDuckActive = false;
    }
}