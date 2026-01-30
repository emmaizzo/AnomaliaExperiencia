using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DoorHandleInteract : MonoBehaviour
{
    public string nextSceneName;

    [Header("Black Fade")]
    public CanvasGroup blackPanel;
    public float fadeDuration = 2f;

    [Header("Audio")]
    public AudioSource audioToFadeOut;     // música / ambiente
    public float audioFadeSpeed = 1f;

    public AudioSource touchAudio;         // 🔊 sonido inmediato al tocar E

    bool playerInside = false;
    bool used = false;

    void Update()
    {
        if (playerInside && !used && Input.GetKeyDown(KeyCode.E))
        {
            used = true;

            // 🔊 sonido instantáneo al interactuar
            if (touchAudio != null)
                touchAudio.Play();

            StartCoroutine(Sequence());
        }
    }

    IEnumerator Sequence()
    {
        // Fade out del audio ambiente
        if (audioToFadeOut != null)
            StartCoroutine(FadeOutAudio());

        // Fade a negro
        yield return StartCoroutine(FadeBlackIn());

        SceneManager.LoadScene(nextSceneName);
    }

    IEnumerator FadeBlackIn()
    {
        float t = 0f;
        blackPanel.alpha = 0f;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            blackPanel.alpha = Mathf.Lerp(0f, 1f, t / fadeDuration);
            yield return null;
        }

        blackPanel.alpha = 1f;
    }

    IEnumerator FadeOutAudio()
    {
        float startVol = audioToFadeOut.volume;
        float t = 0f;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            audioToFadeOut.volume = Mathf.Lerp(startVol, 0f, t / fadeDuration);
            yield return null;
        }

        audioToFadeOut.volume = 0f;
        audioToFadeOut.Stop();
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