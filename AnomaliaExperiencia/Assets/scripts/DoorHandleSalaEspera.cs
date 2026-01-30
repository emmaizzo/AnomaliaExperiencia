using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DoorHandleSalaEspera : MonoBehaviour
{
    [Header("Scene")]
    public string nextSceneName;

    [Header("Fade")]
    public CanvasGroup blackPanel;
    public float fadeDuration = 2f;

    [Header("Audio")]
    public AudioSource handleAudio;

    // 👉 nuevo sonido cuando está bloqueado
    public AudioSource blockedAudio;

    [Header("Interaction")]
    public KeyCode interactionKey = KeyCode.E;

    bool playerInside = false;
    bool used = false;

    void Update()
    {
        if (!playerInside || used)
            return;

        if (Input.GetKeyDown(interactionKey))
        {
            // 🔒 sigue bloqueado
            if (!DoorCloseTrigger.lastAudioFinished)
            {
                if (blockedAudio != null && !blockedAudio.isPlaying)
                    blockedAudio.Play();

                return;
            }

            // ✅ ya se puede usar
            used = true;
            StartCoroutine(Sequence());
        }
    }

    IEnumerator Sequence()
    {
        if (handleAudio != null)
            handleAudio.Play();

        yield return StartCoroutine(FadeToBlack());

        yield return new WaitForSeconds(fadeDuration);

        SceneManager.LoadScene(nextSceneName);
    }

    IEnumerator FadeToBlack()
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