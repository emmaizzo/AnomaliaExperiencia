using System.Collections;
using System.Collections.Generic;
using UnityEngine.Rendering;
using UnityEngine;

public class RisingWater : MonoBehaviour
{
    [Header("Timing")]
    public float soundDelay = 10f;
    public float waterDelay = 15f;

    [Header("Water Movement")]
    public float normalSpeed = 0.2f;
    public float slowSpeed = 0.05f;
    public float slowStartHeight = 2.5f;
    public float maxHeight = 4f;

    [Header("Audio")]
    public AudioSource audioSource;

    [Header("Underwater Volumes")]
    public Volume surfaceVolume;
    public Volume underwaterVolume;
    public Transform playerCamera;
    public float waterOffset = 0.05f;

    bool waterRising = false;
    bool underwater = false;

    void Start()
    {
        surfaceVolume.weight = 1f;
        underwaterVolume.weight = 0f;

        StartCoroutine(SceneSequence());
    }

    IEnumerator SceneSequence()
    {
        yield return new WaitForSeconds(soundDelay);
        if (audioSource != null)
            audioSource.Play();

        yield return new WaitForSeconds(waterDelay - soundDelay);
        waterRising = true;
    }

    void Update()
    {
        HandleWater();
        HandleUnderwater();
    }

    void HandleWater()
    {
        if (!waterRising) return;

        float speed = transform.position.y < slowStartHeight
            ? normalSpeed
            : slowSpeed;

        if (transform.position.y < maxHeight)
            transform.position += Vector3.up * speed * Time.deltaTime;
    }

    void HandleUnderwater()
    {
        if (playerCamera == null) return;

        float waterSurfaceY =
            transform.position.y + (transform.localScale.y * 0.5f);

        bool shouldBeUnderwater =
            playerCamera.position.y < waterSurfaceY + waterOffset;

        if (shouldBeUnderwater && !underwater)
        {
            underwater = true;
            underwaterVolume.weight = 1f;
            surfaceVolume.weight = 0f;
        }
        else if (!shouldBeUnderwater && underwater)
        {
            underwater = false;
            underwaterVolume.weight = 0f;
            surfaceVolume.weight = 0.7f;
        }
    }
}