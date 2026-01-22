using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeartbeatDistanceControl : MonoBehaviour
{
    public Transform player;

    [Header("Distance")]
    public float maxDistance = 60f;
    public float minDistance = 1f;

    [Header("Volume")]
    public float minVolume = 0.5f;
    public float maxVolume = 5f;

    [Header("Pitch")]
    public float minPitch = 0.8f;
    public float maxPitch = 2f;

    AudioSource audioSource;
    bool initialized = false;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        // Fuerza valores iniciales
        audioSource.volume = minVolume;
        audioSource.pitch = minPitch;
    }

    void Update()
    {
        if (!audioSource.isPlaying)
            return;

        UpdateHeartbeat();

        if (!initialized)
            initialized = true;
    }

    void UpdateHeartbeat()
    {
        float distance = Vector3.Distance(player.position, transform.position);
        float t = Mathf.InverseLerp(maxDistance, minDistance, distance);

        audioSource.volume = Mathf.Lerp(minVolume, maxVolume, t);
        audioSource.pitch = Mathf.Lerp(minPitch, maxPitch, t);
    }
}