using System.Collections;
using UnityEngine;

public class CorridorAudioByDistance : MonoBehaviour
{
    [Header("Referencias")]
    public Transform player;

    [Header("Audio Sources")]
    public AudioSource musicA;   // chill
    public AudioSource musicB;
    public AudioSource breathing;

    [Header("Audio extra por distancia")]
    public AudioSource distanceEventAudio;
    public float distanceEventTrigger = 150f;

    [Header("Distancias")]
    public float musicTransitionDistance = 100f;
    public float musicFadeRange = 20f;
    public float fullEffectDistance = 200f;

    [Header("Respiración")]
    public float breathingDelayAfterMusicBStart = 5f;
    public float maxBreathingVolume = 0.4f;
    public float minBreathingPitch = 0.95f;
    public float maxBreathingPitch = 1.25f;

    [Header("Ducking por eventos")]
    public float eventDuckMultiplier = 0.25f;
    public float eventDuckFadeTime = 0.25f;
    public float eventDuckDuration = 3f;

    // -------------------------
    // Delay música chill
    // -------------------------
    [Header("Delay música chill")]
    public float chillStartDelay = 11f;

    float chillTimer = 0f;
    bool chillStarted = false;

    float totalDistance;
    Vector3 lastPlayerPos;

    bool musicBStarted = false;
    bool breathingStarted = false;
    float breathingTimer = 0f;
    bool distanceEventPlayed = false;

    float baseMusicAVol;
    float baseMusicBVol;
    float baseBreathingVol;

    Coroutine duckRoutine;

    void Start()
    {
        lastPlayerPos = player.position;

        baseMusicAVol = 1f;
        baseMusicBVol = 0f;
        baseBreathingVol = 0f;

        musicA.volume = 0f;     // no suena hasta el delay
        musicB.volume = baseMusicBVol;
        breathing.volume = baseBreathingVol;

        // musicA NO se reproduce todavía
        musicB.Play();

        breathing.Play();
        breathing.Pause();
    }

    void Update()
    {
        // --------------------
        // Delay música chill
        // --------------------
        if (!chillStarted)
        {
            chillTimer += Time.deltaTime;

            if (chillTimer >= chillStartDelay)
            {
                chillStarted = true;
                musicA.Play();
            }
        }

        float delta = Vector3.Distance(player.position, lastPlayerPos);
        totalDistance += delta;
        lastPlayerPos = player.position;

        // 🎯 EVENTO A LOS 150
        if (!distanceEventPlayed && totalDistance >= distanceEventTrigger)
        {
            distanceEventPlayed = true;

            if (distanceEventAudio != null)
            {
                distanceEventAudio.Play();
                DuckForEvent();
            }
        }

        float fadeStart = musicTransitionDistance - musicFadeRange;
        float fadeEnd = musicTransitionDistance + musicFadeRange;

        float musicT = Mathf.Clamp01(Mathf.InverseLerp(fadeStart, fadeEnd, totalDistance));

        baseMusicAVol = 1f - musicT;
        baseMusicBVol = musicT;

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

            baseBreathingVol = Mathf.Lerp(0f, maxBreathingVolume, breathT);
            breathing.pitch = Mathf.Lerp(minBreathingPitch, maxBreathingPitch, breathT);
        }

        // --------------------
        // Aplica volúmenes base
        // --------------------
        if (duckRoutine == null)
        {
            if (chillStarted)
                musicA.volume = baseMusicAVol;
            else
                musicA.volume = 0f;

            musicB.volume = baseMusicBVol;
            breathing.volume = baseBreathingVol;
        }
    }

    // =========================
    // DUCKING POR EVENTO
    // =========================

    void DuckForEvent()
    {
        if (duckRoutine != null)
            StopCoroutine(duckRoutine);

        duckRoutine = StartCoroutine(EventDuckRoutine());
    }

    IEnumerator EventDuckRoutine()
    {
        float t = 0f;

        float startA = musicA.volume;
        float startB = musicB.volume;
        float startBreath = breathing.volume;

        // Fade down
        while (t < eventDuckFadeTime)
        {
            t += Time.deltaTime;
            float lerp = t / eventDuckFadeTime;

            musicA.volume = Mathf.Lerp(startA, baseMusicAVol * eventDuckMultiplier, lerp);
            musicB.volume = Mathf.Lerp(startB, baseMusicBVol * eventDuckMultiplier, lerp);
            breathing.volume = Mathf.Lerp(startBreath, baseBreathingVol * eventDuckMultiplier, lerp);

            yield return null;
        }

        // Mantiene duck
        yield return new WaitForSeconds(eventDuckDuration);

        // Fade up
        t = 0f;
        startA = musicA.volume;
        startB = musicB.volume;
        startBreath = breathing.volume;

        while (t < eventDuckFadeTime)
        {
            t += Time.deltaTime;
            float lerp = t / eventDuckFadeTime;

            musicA.volume = Mathf.Lerp(startA, baseMusicAVol, lerp);
            musicB.volume = Mathf.Lerp(startB, baseMusicBVol, lerp);
            breathing.volume = Mathf.Lerp(startBreath, baseBreathingVol, lerp);

            yield return null;
        }

        duckRoutine = null;
    }
}
