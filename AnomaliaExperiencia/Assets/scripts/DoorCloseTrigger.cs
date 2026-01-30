using System.Collections;
using System.Collections.Generic;
using StarterAssets;
using UnityEngine.InputSystem;
using UnityEngine;

public class DoorCloseTrigger : MonoBehaviour
{
    public DoorOpenSmooth door;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip firstAudio;
    public AudioClip middleAudio;
    public AudioClip lastAudio;

    public float delayBeforeFirst = 15f;

    // 👉 NUEVO
    public static bool lastAudioFinished = false;

    bool triggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !triggered)
        {
            triggered = true;

            lastAudioFinished = false;

            // 🔒 Bloqueo REAL del movimiento
            FirstPersonController fpc = other.GetComponent<FirstPersonController>();
            PlayerInput input = other.GetComponent<PlayerInput>();

            if (fpc != null && input != null)
            {
                StartCoroutine(FreezePlayer(fpc, input));
            }

            door.CloseAndLockDoor();
            StartCoroutine(AudioSequence());
        }
    }

    IEnumerator AudioSequence()
    {
        yield return new WaitForSeconds(delayBeforeFirst);

        // Audio 1
        audioSource.clip = firstAudio;
        audioSource.Play();
        yield return new WaitForSeconds(firstAudio.length);

        // Audio 2
        audioSource.clip = middleAudio;
        audioSource.Play();
        yield return new WaitForSeconds(middleAudio.length);

        // Audio 3
        audioSource.clip = lastAudio;
        audioSource.Play();

        // 👉 esperamos a que termine
        yield return new WaitForSeconds(lastAudio.length);

        // 👉 avisamos que ya terminó
        lastAudioFinished = true;
    }

    IEnumerator FreezePlayer(FirstPersonController fpc, PlayerInput input)
    {
        input.enabled = false;
        fpc.enabled = false;

        yield return null;
        yield return null;

        fpc.enabled = true;
        input.enabled = true;
    }
}