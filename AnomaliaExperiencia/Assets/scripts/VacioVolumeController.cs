using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class AnomalousVolumeController : MonoBehaviour
{
    [Header("References")]
    public Volume globalVolume;
    public Transform player;
    public Transform soundSource;

    [Header("Distance")]
    public float maxDistance = 60f;
    public float minDistance = 1.5f;

    [Header("Vignette")]
    public float vignetteMin = 0.25f;
    public float vignetteMax = 0.55f;

    [Header("Film Grain")]
    public float grainMin = 0.18f;
    public float grainMax = 0.45f;

    [Header("Chromatic Aberration")]
    public float chromaMin = 0f;
    public float chromaMax = 0.25f;

    [Header("Saturation")]
    public float saturationMin = -25f;
    public float saturationMax = -60f;

    Vignette vignette;
    FilmGrain grain;
    ChromaticAberration chroma;
    ColorAdjustments color;

    void Start()
    {
        globalVolume.profile.TryGet(out vignette);
        globalVolume.profile.TryGet(out grain);
        globalVolume.profile.TryGet(out chroma);
        globalVolume.profile.TryGet(out color);
    }

    void Update()
    {
        float distance = Vector3.Distance(player.position, soundSource.position);

        float t = Mathf.InverseLerp(maxDistance, minDistance, distance);
        t = Mathf.Pow(t, 3f); // 💥 agresivo cerca

        // VIGNETTE
        vignette.intensity.value = Mathf.Lerp(vignetteMin, vignetteMax, t);

        // FILM GRAIN
        grain.intensity.value = Mathf.Lerp(grainMin, grainMax, t);

        // CHROMATIC ABERRATION
        chroma.intensity.value = Mathf.Lerp(chromaMin, chromaMax, t);

        // COLOR / SATURATION
        color.saturation.value = Mathf.Lerp(saturationMin, saturationMax, t);
    }
}