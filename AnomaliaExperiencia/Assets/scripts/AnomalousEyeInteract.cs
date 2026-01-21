using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnomalousEyeInteract : MonoBehaviour
{
    public float distanciaInteraccion = 2f;
    public CanvasGroup fadeCanvas;
    public float fadeDuracion = 1f;

    bool usado = false;

    void Update()
    {
        if (usado) return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            Transform cam = Camera.main.transform;

            if (Vector3.Distance(cam.position, transform.position) <= distanciaInteraccion)
            {
                StartCoroutine(FadeOut());
                usado = true;
            }
        }
    }

    IEnumerator FadeOut()
    {
        float t = 0f;

        while (t < fadeDuracion)
        {
            t += Time.deltaTime;
            fadeCanvas.alpha = Mathf.Lerp(0, 1, t / fadeDuracion);
            yield return null;
        }

        // acá después cargás escena, movés pared, etc
    }
}