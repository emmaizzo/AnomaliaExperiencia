using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DelayedSpatialSound : MonoBehaviour
{
    public AudioSource spatialAudio;
    public float delay = 11f;

    void Start()
    {
        if (spatialAudio != null)
        {
            spatialAudio.playOnAwake = false;
            spatialAudio.Stop();
        }

        StartCoroutine(PlaySoundAfterDelay());
    }

    IEnumerator PlaySoundAfterDelay()
    {
        yield return new WaitForSeconds(delay);

        if (spatialAudio != null)
        {
            spatialAudio.Play();
        }
    }
}

