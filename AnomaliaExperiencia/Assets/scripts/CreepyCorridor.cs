using System.Collections;
using System.Collections.Generic;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine;

public class CreepyCorridor : MonoBehaviour
{
    [Header("Referencias")]
    public Transform player;
    public Volume globalVolume;

    [Header("Progresión")]
    public float distanceForFullEffect = 200f;

    float totalDistance;
    Vector3 lastPlayerPos;

    // Volume overrides
    ColorAdjustments color;
    Vignette vignette;
    FilmGrain grain;
    Bloom bloom;
    ChromaticAberration chromatic;
    LensDistortion distortion;

    void Start()
    {
        lastPlayerPos = player.position;

        globalVolume.profile.TryGet(out color);
        globalVolume.profile.TryGet(out vignette);
        globalVolume.profile.TryGet(out grain);
        globalVolume.profile.TryGet(out bloom);
        globalVolume.profile.TryGet(out chromatic);
        globalVolume.profile.TryGet(out distortion);
    }

    void Update()
    {
        float delta = Vector3.Distance(player.position, lastPlayerPos);
        totalDistance += delta;
        lastPlayerPos = player.position;

        float t = Mathf.Clamp01(totalDistance / distanceForFullEffect);

        // Color Adjustments
        color.postExposure.value = Mathf.Lerp(0f, -0.6f, t);
        color.contrast.value = Mathf.Lerp(0f, 40f, t);
        color.saturation.value = Mathf.Lerp(0f, -45f, t);

        // Vignette
        vignette.intensity.value = Mathf.Lerp(0f, 0.45f, t);
        vignette.smoothness.value = Mathf.Lerp(0.6f, 0.8f, t);

        // Film Grain
        grain.intensity.value = Mathf.Lerp(0f, 0.35f, t);

        // Bloom
        bloom.intensity.value = Mathf.Lerp(0f, 3f, t);
        bloom.threshold.value = Mathf.Lerp(1.2f, 0.9f, t);

        // Chromatic Aberration
        chromatic.intensity.value = Mathf.Lerp(0f, 0.2f, t);

        // Distorsión opcional
        if (distortion != null)
            distortion.intensity.value = Mathf.Lerp(0f, -0.15f, t);
    }
}