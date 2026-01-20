using System.Collections;
using System.Collections.Generic;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine;

public class TrapdoorTrigger : MonoBehaviour
{
    bool playerInside;
    public AudioSource audioSource;

    [Header("Global Volume")]
    public Volume globalVolume;

    Vignette vignette;
    ColorAdjustments color;
    DepthOfField dof;

    [Header("Stress Visuals")]
    public float maxStressSteps = 5f;
    float currentStress;

    [Header("Volume Transition")]
    public float volumeTransitionTime = 0.6f;
    Coroutine volumeRoutine;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        if (globalVolume != null && globalVolume.profile != null)
        {
            globalVolume.profile.TryGet(out vignette);
            globalVolume.profile.TryGet(out color);
            globalVolume.profile.TryGet(out dof);
        }

        ResetVolumeVisuals();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            playerInside = true;
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            playerInside = false;
    }

    void Update()
    {
        if (playerInside && Input.GetKeyDown(KeyCode.E))
        {
            StressManager.Instance.IncreaseStress();

            currentStress++;
            currentStress = Mathf.Clamp(currentStress, 0, maxStressSteps);

            UpdateVolume();

            if (audioSource != null)
                audioSource.Play();
        }
    }

    void UpdateVolume()
    {
        float t = currentStress / maxStressSteps;

        if (volumeRoutine != null)
            StopCoroutine(volumeRoutine);

        volumeRoutine = StartCoroutine(VolumeTransition(t));
    }

    IEnumerator VolumeTransition(float targetT)
    {
        float startWeight = globalVolume.weight;
        float targetWeight = Mathf.Lerp(0f, 1f, targetT);

        float startVignette = vignette.intensity.value;
        float targetVignette = Mathf.Lerp(0.15f, 0.45f, targetT);

        float startExposure = color.postExposure.value;
        float targetExposure = Mathf.Lerp(0f, -0.8f, targetT);

        float startSaturation = color.saturation.value;
        float targetSaturation = Mathf.Lerp(0f, -25f, targetT);

        float startContrast = color.contrast.value;
        float targetContrast = Mathf.Lerp(0f, 15f, targetT);

        float startBlur = dof.gaussianEnd.value;
        float targetBlur = Mathf.Lerp(3f, 1.2f, targetT);

        float t = 0f;

        while (t < volumeTransitionTime)
        {
            t += Time.deltaTime;
            float lerp = t / volumeTransitionTime;

            globalVolume.weight = Mathf.Lerp(startWeight, targetWeight, lerp);
            vignette.intensity.value = Mathf.Lerp(startVignette, targetVignette, lerp);
            color.postExposure.value = Mathf.Lerp(startExposure, targetExposure, lerp);
            color.saturation.value = Mathf.Lerp(startSaturation, targetSaturation, lerp);
            color.contrast.value = Mathf.Lerp(startContrast, targetContrast, lerp);
            dof.gaussianEnd.value = Mathf.Lerp(startBlur, targetBlur, lerp);

            yield return null;
        }
    }

    public void ResetVolumeVisuals()
    {
        currentStress = 0f;

        if (volumeRoutine != null)
            StopCoroutine(volumeRoutine);

        if (globalVolume != null)
            globalVolume.weight = 0f;

        if (vignette != null)
            vignette.intensity.value = 0.15f;

        if (color != null)
        {
            color.postExposure.value = 0f;
            color.saturation.value = 0f;
            color.contrast.value = 0f;
        }

        if (dof != null)
            dof.gaussianEnd.value = 3f;
    }
}