using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine;

public class DoorTransition : MonoBehaviour
{
    [Header("Referencias")]
    public FadeController fadeController;
    public AudioFadeController audioFade;
    public AudioSource doorSound;

    [Header("Player Footsteps")]
    public AudioSource footstepAudio; // 👈 arrastrar audio de pasos acá

    [Header("Audio durante black screen")]
    public AudioSource blackScreenAudio;
    public float blackScreenAudioDelay = 1f;

    [Header("Black screen timing")]
    public float blackScreenDuration = 8f;

    [Header("Escena")]
    public string nextSceneName;

    bool playerInside = false;
    bool triggered = false;

    void Update()
    {
        if (playerInside && !triggered && Input.GetKeyDown(KeyCode.E))
        {
            triggered = true;
            StartCoroutine(TransitionSequence());
        }
    }

    IEnumerator TransitionSequence()
    {
        // 🔊 Sonido de puerta
        if (doorSound != null)
            doorSound.Play();

        // 🔇 Silenciar pasos
        if (footstepAudio != null)
            footstepAudio.mute = true;

        // 🔉 Fade out de audios
        if (audioFade != null)
            audioFade.FadeOutAll();

        // 🖤 Fade a negro
        if (fadeController != null)
            fadeController.FadeToBlack(null);

        // 🎙️ Audio al segundo 1 del negro
        if (blackScreenAudio != null)
        {
            yield return new WaitForSeconds(blackScreenAudioDelay);
            blackScreenAudio.Play();
        }

        // ⏱️ Mantiene el negro
        yield return new WaitForSeconds(
            blackScreenDuration - blackScreenAudioDelay
        );

        // 🚪 Carga escena
        SceneManager.LoadScene(nextSceneName);
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