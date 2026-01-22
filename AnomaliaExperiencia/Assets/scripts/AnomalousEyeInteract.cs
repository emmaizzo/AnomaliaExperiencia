using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AnomalousEyeInteract : MonoBehaviour
{
    public float distanciaInteraccion = 2f;

    [Header("Fade")]
    public CanvasGroup fadeCanvas;
    public float fadeDuracion = 1f;

    [Header("Escena")]
    public string nombreEscena;

    bool usado = false;

    void Update()
    {
        if (usado) return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            Transform cam = Camera.main.transform;

            if (Vector3.Distance(cam.position, transform.position) <= distanciaInteraccion)
            {
                usado = true;
                StartCoroutine(FadeOutYEscena());
            }
        }
    }

    IEnumerator FadeOutYEscena()
    {
        float t = 0f;

        fadeCanvas.blocksRaycasts = true;

        while (t < fadeDuracion)
        {
            t += Time.deltaTime;
            fadeCanvas.alpha = Mathf.Lerp(0f, 1f, t / fadeDuracion);
            yield return null;
        }

        fadeCanvas.alpha = 1f;

        // cargar escena
        SceneManager.LoadScene(nombreEscena);
    }
}