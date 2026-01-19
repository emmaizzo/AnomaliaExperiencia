using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CorridorAudioByDistance : MonoBehaviour
{
    [Header("Referencias")]
    public Transform player;

    [Header("Audio Sources")]
    public AudioSource musicA;
    public AudioSource musicB;
    public AudioSource breathing;

    [Header("Distancias")]
    public float musicTransitionDistance = 100f;
    public float musicFadeRange = 20f;
    public float fullEffectDistance = 200f;

    [Header("Respiración")]
    public float breathingDelayAfterMusicBStart = 5f;
    public float maxBreathingVolume = 0.4f;
    public float minBreathingPitch = 0.95f;
    public float maxBreathingPitch = 1.25f;

    float totalDistance;
    Vector3 lastPlayerPos;

    bool musicBStarted = false;
    bool breathingStarted = false;
    float breathingTimer = 0f;

    void Start()
    {
        lastPlayerPos = player.position;

        musicA.volume = 1f;
        musicB.volume = 0f;
        breathing.volume = 0f;

        musicA.Play();
        musicB.Play();

        breathing.Play();
        breathing.Pause();
    }

    void Update()
    {
        float delta = Vector3.Distance(player.position, lastPlayerPos);
        totalDistance += delta;
        lastPlayerPos = player.position;

        float fadeStart = musicTransitionDistance - musicFadeRange;
        float fadeEnd = musicTransitionDistance + musicFadeRange;

        float musicT = Mathf.InverseLerp(fadeStart, fadeEnd, totalDistance);
        musicT = Mathf.Clamp01(musicT);

        musicA.volume = 1f - musicT;
        musicB.volume = musicT;

        if (!musicBStarted && musicT > 0.01f)
        {
            musicBStarted = true;
            breathingTimer = 0f;
        }

        if (musicBStarted && !breathingStarted)
        {
            breathingTimer += Time.deltaTime;

            if (breathingTimer >= breathingDelayAfterMusicBStart)
            {
                breathingStarted = true;
                breathing.UnPause();
            }
        }

        if (breathingStarted)
        {
            float breathT = Mathf.InverseLerp(
                musicTransitionDistance,
                fullEffectDistance,
                totalDistance
            );

            breathing.volume = Mathf.Lerp(0f, maxBreathingVolume, breathT);
            breathing.pitch = Mathf.Lerp(minBreathingPitch, maxBreathingPitch, breathT);
        }
    }
}