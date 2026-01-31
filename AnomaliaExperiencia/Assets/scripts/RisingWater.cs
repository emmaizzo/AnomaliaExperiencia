using System.Collections;
using System.Collections.Generic;
using UnityEngine.Rendering;
using UnityEngine;

public class RisingWater : MonoBehaviour
{
    [Header("TIMING (seconds)")]
    public float chillWaterStartTime = 5f;
    public float waterRiseStartTime = 15f;
    public float strongWaterStartTime = 30f;
    public float objectAppearTime = 90f;

    [Header("Water Movement")]
    public float normalSpeed = 0.2f;
    public float slowSpeed = 0.05f;
    public float slowStartHeight = 2.5f;
    public float maxHeight = 4f;

    [Header("Audio")]
    public AudioSource chillWaterAudio;
    public AudioSource strongWaterAudio;
    public float audioFadeSpeed = 1f;

    [Header("Underwater Volumes")]
    public Volume surfaceVolume;
    public Volume underwaterVolume;
    public float waterOffset = 0.05f;
    public float volumeLerpSpeed = 2f;

    [Header("Rising Object")]
    public Transform risingObject;
    public float objectRiseSpeed = 0.5f;
    public Vector3 objectTargetOffset = Vector3.up * 1.5f;
    public AudioSource objectAppearAudio;

    // 🌱 PARTICULAS PLANTA
    [Header("Plant Particles")]
    public ParticleSystem[] plantParticles;

    // 🌊 PARTICULAS DE AGUA
    [Header("Water Particles")]
    public float waterParticlesStartTime = 25f;
    public ParticleSystem[] waterParticles;

    // ---------------- INTRO BLACK SCREEN ----------------
    [Header("Intro Black Screen")]
    public CanvasGroup blackScreen;
    public float blackScreenDuration = 7f;
    public AudioSource blackScreenSecondAudio;
    public AudioSource blackScreenEndAudio;

    // ---------------- PLAYER LOCK ----------------
    [Header("Player Lock")]
    public MonoBehaviour playerController;
    public AudioSource playerFootstepsAudio;

    // ---------------- EXTRA SFX ----------------
    [Header("Extra SFX")]
    public AudioSource waterStartAudio;
    public float waterStartAudioDelay = 1.5f;
    public AudioSource plantFinishedAudio;
    public AudioSource afterPlantFinishedAudio;

    // ---------------- UNDERWATER SFX ----------------
    [Header("Underwater SFX")]
    public AudioSource drowningAudio;
    public float drowningFadeSpeed = 2f;

    // ---------------- TIMED SFX ----------------
    [Header("Timed SFX")]
    public AudioSource midSceneAudio;
    public float midSceneStartTime = 45f;
    public float midScenePitchSpeed = 0.15f;
    public float midSceneMaxPitch = 2f;

    public WaterPlayerPhysics waterPlayerPhysics;

    float sceneTimer;
    bool waterRising;
    bool chillStarted;
    bool strongStarted;
    bool objectRising;

    bool waterStartSoundPlayed;
    bool plantFinishedSoundPlayed;
    bool midSceneStarted;
    bool afterPlantStarted;

    bool roomAudioStopped;
    bool waterParticlesStarted;

    Vector3 objectStartPos;
    Vector3 objectTargetPos;

    void Start()
    {
        surfaceVolume.weight = 1f;
        underwaterVolume.weight = 0f;

        // apagar partículas de planta
        if (plantParticles != null)
            foreach (var ps in plantParticles)
                if (ps) ps.Stop();

        // apagar partículas de agua
        if (waterParticles != null)
            foreach (var ps in waterParticles)
                if (ps) ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

        if (chillWaterAudio)
        {
            chillWaterAudio.loop = true;
            chillWaterAudio.volume = 1f;
        }

        if (strongWaterAudio)
        {
            strongWaterAudio.loop = true;
            strongWaterAudio.volume = 0f;
            strongWaterAudio.Play();
        }

        if (risingObject)
        {
            objectStartPos = risingObject.position;
            objectTargetPos = objectStartPos + objectTargetOffset;
        }

        if (drowningAudio)
        {
            drowningAudio.loop = true;
            drowningAudio.volume = 0f;
            drowningAudio.Play();
        }

        if (blackScreen)
            StartCoroutine(BlackScreenRoutine());
    }

    void Update()
    {
        if (roomAudioStopped) return;

        sceneTimer += Time.deltaTime;

        HandleAudioTimeline();
        HandleWater();
        HandleUnderwater();
        HandleRisingObject();
        HandleMidSceneAudio();
        HandleMidScenePitch();
        HandleWaterParticles();
    }

    // ---------------- BLACK SCREEN ----------------

    IEnumerator BlackScreenRoutine()
    {
        if (playerController) playerController.enabled = false;
        if (playerFootstepsAudio) playerFootstepsAudio.mute = true;

        blackScreen.alpha = 1f;
        blackScreen.blocksRaycasts = true;

        yield return new WaitForSeconds(1f);

        blackScreenSecondAudio?.Play();

        yield return new WaitForSeconds(blackScreenDuration - 1f);

        blackScreen.alpha = 0f;
        blackScreen.blocksRaycasts = false;

        blackScreenEndAudio?.Play();

        if (playerController) playerController.enabled = true;
        if (playerFootstepsAudio) playerFootstepsAudio.mute = false;
    }

    IEnumerator PlayWaterStartDelayed()
    {
        yield return new WaitForSeconds(waterStartAudioDelay);
        waterStartAudio?.Play();
    }

    // ---------------- AUDIO TIMELINE ----------------

    void HandleAudioTimeline()
    {
        if (!chillStarted && sceneTimer >= chillWaterStartTime)
        {
            chillStarted = true;
            chillWaterAudio?.Play();
        }

        if (!strongStarted && sceneTimer >= strongWaterStartTime)
            strongStarted = true;

        if (strongStarted)
        {
            if (chillWaterAudio)
                chillWaterAudio.volume = Mathf.MoveTowards(
                    chillWaterAudio.volume, 0f, Time.deltaTime * audioFadeSpeed);

            if (strongWaterAudio)
                strongWaterAudio.volume = Mathf.MoveTowards(
                    strongWaterAudio.volume, 1f, Time.deltaTime * audioFadeSpeed);
        }
    }

    // ---------------- WATER ----------------

    void HandleWater()
    {
        if (!waterRising && sceneTimer >= waterRiseStartTime)
        {
            waterRising = true;

            if (!waterStartSoundPlayed)
            {
                waterStartSoundPlayed = true;
                StartCoroutine(PlayWaterStartDelayed());
            }
        }

        if (!waterRising) return;

        float speed = transform.position.y < slowStartHeight ? normalSpeed : slowSpeed;

        if (transform.position.y < maxHeight)
            transform.position += Vector3.up * speed * Time.deltaTime;
    }

    // ---------------- WATER PARTICLES ----------------

    void HandleWaterParticles()
    {
        if (!waterParticlesStarted && sceneTimer >= waterParticlesStartTime)
        {
            waterParticlesStarted = true;

            if (waterParticles != null)
                foreach (var ps in waterParticles)
                    if (ps) ps.Play();
        }

        if (!waterParticlesStarted) return;

        float waterY = transform.position.y;

        if (waterParticles == null) return;

        foreach (var ps in waterParticles)
        {
            if (!ps || !ps.isPlaying) continue;

            if (waterY > ps.transform.position.y)
            {
                ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            }
        }
    }

    // ---------------- PLANTA ----------------

    void HandleRisingObject()
    {
        if (!risingObject) return;

        if (!objectRising && sceneTimer >= objectAppearTime)
        {
            objectRising = true;
            objectAppearAudio?.Play();

            if (plantParticles != null)
                foreach (var ps in plantParticles)
                    if (ps) ps.Play();
        }

        if (!objectRising) return;

        risingObject.position = Vector3.MoveTowards(
            risingObject.position,
            objectTargetPos,
            objectRiseSpeed * Time.deltaTime
        );

        if (!plantFinishedSoundPlayed &&
            Vector3.Distance(risingObject.position, objectTargetPos) < 0.01f)
        {
            plantFinishedSoundPlayed = true;
            plantFinishedAudio?.Play();

            if (plantParticles != null)
                foreach (var ps in plantParticles)
                    if (ps) ps.Stop();

            if (!afterPlantStarted)
                StartCoroutine(PlayAfterPlantFinished());
        }
    }

    IEnumerator PlayAfterPlantFinished()
    {
        afterPlantStarted = true;

        if (plantFinishedAudio?.clip != null)
            yield return new WaitForSeconds(plantFinishedAudio.clip.length);

        afterPlantFinishedAudio?.Play();
    }

    // ---------------- UNDERWATER ----------------

    void HandleUnderwater()
    {
        if (!Camera.main) return;

        float waterY = transform.position.y;
        float camY = Camera.main.transform.position.y;

        float target = camY < waterY + waterOffset ? 1f : 0f;

        if (waterPlayerPhysics)
            waterPlayerPhysics.SetUnderwater(target > 0.5f);

        underwaterVolume.weight = Mathf.Lerp(
            underwaterVolume.weight, target, Time.deltaTime * volumeLerpSpeed);

        surfaceVolume.weight = 1f - underwaterVolume.weight;

        if (drowningAudio)
        {
            float targetVol = target > 0.5f ? 1f : 0f;

            drowningAudio.volume = Mathf.MoveTowards(
                drowningAudio.volume,
                targetVol,
                Time.deltaTime * drowningFadeSpeed
            );

            if (targetVol == 1f && !drowningAudio.isPlaying)
                drowningAudio.Play();

            if (targetVol == 0f && drowningAudio.volume <= 0.01f)
                drowningAudio.Stop();
        }
    }

    // ---------------- MID AUDIO ----------------

    void HandleMidSceneAudio()
    {
        if (midSceneStarted) return;

        if (sceneTimer >= midSceneStartTime)
        {
            midSceneStarted = true;

            if (midSceneAudio)
            {
                midSceneAudio.loop = true;
                midSceneAudio.pitch = 1f;
                midSceneAudio.Play();
            }
        }
    }

    void HandleMidScenePitch()
    {
        if (!midSceneStarted || !midSceneAudio) return;

        midSceneAudio.pitch = Mathf.MoveTowards(
            midSceneAudio.pitch,
            midSceneMaxPitch,
            Time.deltaTime * (midScenePitchSpeed * 0.25f));
    }

    // ---------------- STOP ALL AUDIO ----------------

    public void StopAllRoomAudio()
    {
        roomAudioStopped = true;

        StopAllCoroutines();

        AudioSource[] audios =
        {
            chillWaterAudio,
            strongWaterAudio,
            waterStartAudio,
            plantFinishedAudio,
            afterPlantFinishedAudio,
            objectAppearAudio,
            drowningAudio,
            midSceneAudio
        };

        foreach (var a in audios)
        {
            if (!a) continue;
            a.Stop();
            a.volume = 0f;
        }
    }
}