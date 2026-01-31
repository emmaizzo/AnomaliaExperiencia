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
    public AudioSource blockedAudio;

    [Header("Player Footsteps")]
    public AudioSource footstepAudio; // 👈 arrastrá acá el audio de pasos del player

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
            if (!DoorCloseTrigger.lastAudioFinished)
            {
                if (blockedAudio != null && !blockedAudio.isPlaying)
                    blockedAudio.Play();

                return;
            }

            used = true;
            StartCoroutine(Sequence());
        }
    }

    IEnumerator Sequence()
    {
        if (handleAudio != null)
            handleAudio.Play();

        // 🔇 apagar pasos
        if (footstepAudio != null)
            footstepAudio.mute = true;

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