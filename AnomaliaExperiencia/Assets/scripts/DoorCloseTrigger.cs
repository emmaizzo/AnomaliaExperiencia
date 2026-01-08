using System.Collections;
using System.Collections.Generic;
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

    bool triggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !triggered)
        {
            triggered = true;

            door.CloseAndLockDoor();
            StartCoroutine(AudioSequence());
        }
    }

    IEnumerator AudioSequence()
    {
        // Espera inicial
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
    }
}