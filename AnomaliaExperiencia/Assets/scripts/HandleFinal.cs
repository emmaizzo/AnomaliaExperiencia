using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HandleFinal : MonoBehaviour
{
    [Header("Interaction")]
    public KeyCode interactionKey = KeyCode.E;
    bool playerInside = false;
    bool used = false;

    [Header("Audio")]
    public AudioSource handleAudio;   // sonido inmediato
    public AudioSource audioToPlay;   // audio con delay
    public float audioDelay = 2f;

    [Header("White Image")]
    public CanvasGroup whiteImageCanvas;
    public float whiteFadeDuration = 0.5f;

    [Header("Top Image")]
    public CanvasGroup topImageCanvas;
    public float topImageDelay = 1f;
    public float topFadeDuration = 0.5f;

    void Start()
    {
        if (whiteImageCanvas != null)
            whiteImageCanvas.alpha = 0f;

        if (topImageCanvas != null)
            topImageCanvas.alpha = 0f;
    }

    void Update()
    {
        if (playerInside && !used && Input.GetKeyDown(interactionKey))
        {
            used = true;

            if (handleAudio != null)
                handleAudio.Play();

            StartCoroutine(PlaySequence());
        }
    }

    IEnumerator PlaySequence()
    {
        // aparece la imagen blanca primero
        if (whiteImageCanvas != null)
            yield return StartCoroutine(FadeCanvas(whiteImageCanvas, 0f, 1f, whiteFadeDuration));

        // espera 1 segundo
        yield return new WaitForSeconds(topImageDelay);

        // aparece la imagen de arriba
        if (topImageCanvas != null)
            yield return StartCoroutine(FadeCanvas(topImageCanvas, 0f, 1f, topFadeDuration));

        // delay del audio principal
        yield return new WaitForSeconds(audioDelay);

        if (audioToPlay != null)
            audioToPlay.Play();
    }

    IEnumerator FadeCanvas(CanvasGroup cg, float from, float to, float duration)
    {
        if (cg == null) yield break;

        float t = 0f;
        cg.alpha = from;

        while (t < duration)
        {
            t += Time.deltaTime;
            cg.alpha = Mathf.Lerp(from, to, t / duration);
            yield return null;
        }

        cg.alpha = to;
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
}