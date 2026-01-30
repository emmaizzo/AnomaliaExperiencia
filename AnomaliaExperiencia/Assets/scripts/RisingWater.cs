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

    // ----------------------------
    // INTRO BLACK SCREEN
    // ----------------------------
    [Header("Intro Black Screen")]
    public CanvasGroup blackScreen;
    public float blackScreenDuration = 7f;
    public AudioSource blackScreenSecondAudio;
    public AudioSource blackScreenEndAudio;

    // ----------------------------
    // PLAYER LOCK
    // ----------------------------
    [Header("Player Lock")]
    public MonoBehaviour playerController;
    public AudioSource playerFootstepsAudio;

    // ----------------------------
    // EXTRA SFX
    // ----------------------------
    [Header("Extra SFX")]
    public AudioSource waterStartAudio;
    public float waterStartAudioDelay = 1.5f;
    public AudioSource plantFinishedAudio;
    public AudioSource afterPlantFinishedAudio;

    // ----------------------------
    // UNDERWATER SFX
    // ----------------------------
    [Header("Underwater SFX")]
    public AudioSource drowningAudio;
    public float drowningFadeSpeed = 2f;

    // ----------------------------
    // TIMED SFX
    // ----------------------------
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

    // 🔴 NUEVO
    bool roomAudioStopped;

    Vector3 objectStartPos;
    Vector3 objectTargetPos;

    void Start()
    {
        surfaceVolume.weight = 1f;
        underwaterVolume.weight = 0f;

        if (chillWaterAudio != null)
        {
            chillWaterAudio.loop = true;
            chillWaterAudio.volume = 1f;
        }

        if (strongWaterAudio != null)
        {
            strongWaterAudio.loop = true;
            strongWaterAudio.volume = 0f;
            strongWaterAudio.Play();
        }

        if (risingObject != null)
        {
            objectStartPos = risingObject.position;
            objectTargetPos = objectStartPos + objectTargetOffset;
        }

        if (drowningAudio != null)
        {
            drowningAudio.loop = true;
            drowningAudio.volume = 0f;
            drowningAudio.Play();
        }

        if (blackScreen != null)
            StartCoroutine(BlackScreenRoutine());
    }

    void Update()
    {
        if (roomAudioStopped)
            return;

        sceneTimer += Time.deltaTime;

        HandleAudioTimeline();
        HandleWater();
        HandleUnderwater();
        HandleRisingObject();
        HandleMidSceneAudio();
        HandleMidScenePitch();
    }

    IEnumerator BlackScreenRoutine()
    {
        if (playerController != null)
            playerController.enabled = false;

        if (playerFootstepsAudio != null)
            playerFootstepsAudio.mute = true;

        blackScreen.alpha = 1f;
        blackScreen.blocksRaycasts = true;

        yield return new WaitForSeconds(1f);

        blackScreenSecondAudio?.Play();

        yield return new WaitForSeconds(blackScreenDuration - 1f);

        blackScreen.alpha = 0f;
        blackScreen.blocksRaycasts = false;

        blackScreenEndAudio?.Play();

        if (playerController != null)
            playerController.enabled = true;

        if (playerFootstepsAudio != null)
            playerFootstepsAudio.mute = false;
    }

    IEnumerator PlayWaterStartDelayed()
    {
        yield return new WaitForSeconds(waterStartAudioDelay);
        waterStartAudio?.Play();
    }

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
            if (chillWaterAudio != null)
                chillWaterAudio.volume = Mathf.MoveTowards(
                    chillWaterAudio.volume, 0f, Time.deltaTime * audioFadeSpeed);

            if (strongWaterAudio != null)
                strongWaterAudio.volume = Mathf.MoveTowards(
                    strongWaterAudio.volume, 1f, Time.deltaTime * audioFadeSpeed);
        }
    }

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

    void HandleRisingObject()
    {
        if (risingObject == null) return;

        if (!objectRising && sceneTimer >= objectAppearTime)
        {
            objectRising = true;
            objectAppearAudio?.Play();
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

            if (plantFinishedAudio != null && !afterPlantStarted)
                StartCoroutine(PlayAfterPlantFinished());
        }
    }

    IEnumerator PlayAfterPlantFinished()
    {
        afterPlantStarted = true;

        if (plantFinishedAudio != null && plantFinishedAudio.clip != null)
            yield return new WaitForSeconds(plantFinishedAudio.clip.length);

        afterPlantFinishedAudio?.Play();
    }

    void HandleUnderwater()
    {
        if (Camera.main == null) return;

        float waterSurfaceY = transform.position.y;
        float camY = Camera.main.transform.position.y;

        float target = camY < waterSurfaceY + waterOffset ? 1f : 0f;

        if (waterPlayerPhysics != null)
            waterPlayerPhysics.SetUnderwater(target > 0.5f);

        underwaterVolume.weight = Mathf.Lerp(
            underwaterVolume.weight,
            target,
            Time.deltaTime * volumeLerpSpeed
        );

        surfaceVolume.weight = 1f - underwaterVolume.weight;

        if (drowningAudio != null)
        {
            float targetVol = target > 0.5f ? 1f : 0f;

            drowningAudio.volume = Mathf.MoveTowards(
                drowningAudio.volume,
                targetVol,
                Time.deltaTime * drowningFadeSpeed
            );

            if (targetVol == 0f && drowningAudio.volume <= 0.01f)
            {
                if (drowningAudio.isPlaying)
                    drowningAudio.Stop();
            }

            if (targetVol == 1f && !drowningAudio.isPlaying)
            {
                drowningAudio.Play();
            }
        }
    }

    void HandleMidSceneAudio()
    {
        if (midSceneStarted) return;

        if (sceneTimer >= midSceneStartTime)
        {
            midSceneStarted = true;

            if (midSceneAudio != null)
            {
                midSceneAudio.loop = true;
                midSceneAudio.pitch = 1f;
                midSceneAudio.Play();
            }
        }
    }

    void HandleMidScenePitch()
    {
        if (!midSceneStarted) return;
        if (midSceneAudio == null) return;
        if (!midSceneAudio.isPlaying) return;

        midSceneAudio.pitch = Mathf.MoveTowards(
            midSceneAudio.pitch,
            midSceneMaxPitch,
            Time.deltaTime * (midScenePitchSpeed * 0.25f)
        );
    }

    // 🔴 se llama desde InteractToChangeScene
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
            if (a == null) continue;

            a.Stop();
            a.volume = 0f;
        }
    }
}