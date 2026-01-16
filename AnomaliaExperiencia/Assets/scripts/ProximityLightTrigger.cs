using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProximityLightTrigger : MonoBehaviour
{
    [Header("Light")]
    public Light targetLight;
    public float lightFadeDuration = 1f;
    public float lightIntensityOn = 1.5f;

    Coroutine lightCoroutine;

    void Start()
    {
        if (targetLight == null)
        {
            Debug.LogError("No asignaste una Light en el Inspector", this);
            return;
        }

        targetLight.intensity = 0f;
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        FadeLight(lightIntensityOn);
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        FadeLight(0f);
    }

    void FadeLight(float to)
    {
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