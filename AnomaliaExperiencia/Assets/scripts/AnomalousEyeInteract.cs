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

    // 🔊 AUDIO DURANTE BLACK SCREEN (AGREGADO)
    [Header("Audio Black Screen")]
    public AudioSource audioBlackScreen;
    public float delayAudioBlackScreen = 2f;

    // ⏱️ DURACIÓN TOTAL DEL NEGRO (AGREGADO)
    public float duracionBlackScreen = 8f;

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

        // 🖤 FADE A NEGRO + FADE DE MÚSICA
        while (t < fadeDuracion)
        {
            t += Time.deltaTime;

            fadeCanvas.alpha = Mathf.Lerp(0f, 1f, t / fadeDuracion);

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

        // ⏱️ ESPERA 2s Y REPRODUCE AUDIO
        if (audioBlackScreen != null)
        {
            yield return new WaitForSeconds(delayAudioBlackScreen);
            audioBlackScreen.Play();
        }

        // ⏱️ ESPERA EL RESTO HASTA COMPLETAR 8s DE NEGRO
        float tiempoRestante = duracionBlackScreen - delayAudioBlackScreen;
        if (tiempoRestante > 0f)
            yield return new WaitForSeconds(tiempoRestante);

        // 🚪 CAMBIO DE ESCENA
        SceneManager.LoadScene(nombreEscena);
    }
}