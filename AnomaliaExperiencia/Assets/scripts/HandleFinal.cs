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
    public AudioSource handleAudio;   // 🔊 sonido inmediato manija
    public AudioSource audioToPlay;   // 🔊 audio con delay
    public float audioDelay = 2f;

    [Header("Image Fade")]
    public CanvasGroup imageCanvas;
    public float fadeDuration = 1.5f;

    void Update()
    {
        if (playerInside && !used && Input.GetKeyDown(interactionKey))
        {
            used = true;

            // 🔊 sonido inmediato de manija
            if (handleAudio != null)
                handleAudio.Play();

            StartCoroutine(PlaySequence());
        }
    }

    IEnumerator PlaySequence()
    {
        // ⏱ delay
        yield return new WaitForSeconds(audioDelay);

        // 🔊 audio principal
        if (audioToPlay != null)
            audioToPlay.Play();

        // 🖼️ fade in imagen
        yield return StartCoroutine(FadeImage(0, 1));
    }

    IEnumerator FadeImage(float from, float to)
    {
        float t = 0f;
        imageCanvas.alpha = from;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            imageCanvas.alpha = Mathf.Lerp(from, to, t / fadeDuration);
            yield return null;
        }

        imageCanvas.alpha = to;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = false;
        }
    }
}