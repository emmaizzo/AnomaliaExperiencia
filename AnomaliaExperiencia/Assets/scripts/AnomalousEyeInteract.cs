using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AnomalousEyeInteract : MonoBehaviour
{
    public float distanciaInteraccion = 2f;

    [Header("Fade Pantalla")]
    public CanvasGroup fadeCanvas;
    public float fadeDuracion = 1f;

    [Header("Música")]
    public AudioSource musicaAmbiente;
    public float duracionFadeMusica = 1.5f;

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
                StartCoroutine(FadeOutCompleto());
            }
        }
    }

    IEnumerator FadeOutCompleto()
    {
        fadeCanvas.blocksRaycasts = true;

        float t = 0f;
        float volumenInicial = musicaAmbiente != null ? musicaAmbiente.volume : 0f;

        while (t < fadeDuracion)
        {
            t += Time.deltaTime;

            // Fade pantalla
            fadeCanvas.alpha = Mathf.Lerp(0f, 1f, t / fadeDuracion);

            // Fade música
            if (musicaAmbiente != null)
            {
                musicaAmbiente.volume = Mathf.Lerp(
                    volumenInicial,
                    0f,
                    t / duracionFadeMusica
                );
            }

            yield return null;
        }

        fadeCanvas.alpha = 1f;

        if (musicaAmbiente != null)
            musicaAmbiente.volume = 0f;

        SceneManager.LoadScene(nombreEscena);
    }
}