using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayAudioAfterDelay : MonoBehaviour
{
    public AudioSource audioSource;
    public float delay = 5f;

    void Start()
    {
        StartCoroutine(PlayAfterDelay());
    }

    IEnumerator PlayAfterDelay()
    {
        yield return new WaitForSeconds(delay);

        if (audioSource != null)
            audioSource.Play();
    }
}