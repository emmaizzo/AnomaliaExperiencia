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

    float sceneTimer;
    bool waterRising;
    bool chillStarted;
    bool strongStarted;
    bool objectRising;

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
    }

    void Update()
    {
        sceneTimer += Time.deltaTime;

        HandleAudioTimeline();
        HandleWater();
        HandleUnderwater();
        HandleRisingObject();
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
                chillWaterAudio.volume = Mathf.MoveTowards(chillWaterAudio.volume, 0f, Time.deltaTime * audioFadeSpeed);

            if (strongWaterAudio != null)
                strongWaterAudio.volume = Mathf.MoveTowards(strongWaterAudio.volume, 1f, Time.deltaTime * audioFadeSpeed);
        }
    }

    void HandleWater()
    {
        if (!waterRising && sceneTimer >= waterRiseStartTime)
            waterRising = true;

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
            objectAppearAudio?.Play(); // 🔊 sonido al salir
        }

        if (!objectRising) return;

        risingObject.position = Vector3.MoveTowards(
            risingObject.position,
            objectTargetPos,
            objectRiseSpeed * Time.deltaTime
        );
    }

    void HandleUnderwater()
    {
        if (Camera.main == null) return;

        float waterSurfaceY = transform.position.y;
        float camY = Camera.main.transform.position.y;

        float target = camY < waterSurfaceY + waterOffset ? 1f : 0f;

        underwaterVolume.weight = Mathf.Lerp(
            underwaterVolume.weight,
            target,
            Time.deltaTime * volumeLerpSpeed
        );

        surfaceVolume.weight = 1f - underwaterVolume.weight;
    }
}