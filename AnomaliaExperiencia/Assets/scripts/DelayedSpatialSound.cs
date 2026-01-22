using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DelayedSpatialSound : MonoBehaviour
{
    public AudioSource spatialAudio;
    public float delay = 10f;

    void Start()
    {
        spatialAudio.playOnAwake = false;
        spatialAudio.loop = true;
        spatialAudio.volume = 0.5f; // volumen inicial
        spatialAudio.pitch = 0.8f;  // latido lento inicial

        StartCoroutine(PlaySoundAfterDelay());
    }

    IEnumerator PlaySoundAfterDelay()
    {
        yield return new WaitForSeconds(delay);

        spatialAudio.volume = 0.5f; // 🔥 fuerza volumen
        spatialAudio.Play();
    }
}