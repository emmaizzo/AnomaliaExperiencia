using System.Collections;
using UnityEngine;

public class ProximityRotate : MonoBehaviour
{
    [Header("Rotation")]
    public Transform targetObject;
    public float rotationSpeed = 100f;

    [Header("Light")]
    public Light targetLight;
    public float lightFadeDuration = 1f;
    public float lightIntensityOn = 1.5f;

    bool playerInside = false;
    Coroutine lightCoroutine;

    void Start()
    {
        // Luz empieza apagada
        if (targetLight != null)
            targetLight.intensity = 0f;
    }

    void Update()
    {
        if (playerInside && targetObject != null)
        {
            targetObject.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        playerInside = true;
        FadeLight(lightIntensityOn);
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        playerInside = false;
        FadeLight(0f);
    }

    void FadeLight(float to)
    {
        if (targetLight == null) return;

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