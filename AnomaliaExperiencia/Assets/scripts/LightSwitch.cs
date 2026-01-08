using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightSwitch : MonoBehaviour
{
    [Header("Switch lever")]
    public Transform lever;
    public float leverAngle = 25f;
    public float leverSpeed = 8f;

    [Header("Lights")]
    public Light[] lights;

    [Header("Emission Materials")]
    public Material[] emissionMaterials;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip switchSound;

    private bool lightsOn = true;
    private bool playerInside = false;
    private bool isAnimating = false;

    private Quaternion leverOnRotation;
    private Quaternion leverOffRotation;

    void Start()
    {
        leverOnRotation = lever.localRotation;

        leverOffRotation = Quaternion.Euler(
            lever.localEulerAngles.x + leverAngle,
            lever.localEulerAngles.y,
            lever.localEulerAngles.z
        );
    }

    void Update()
    {
        if (playerInside && Input.GetKeyDown(KeyCode.E) && !isAnimating)
        {
            ToggleSwitch();
        }
    }

    void ToggleSwitch()
    {
        lightsOn = !lightsOn;

        foreach (Light l in lights)
            l.enabled = lightsOn;

        foreach (Material mat in emissionMaterials)
        {
            if (lightsOn)
                mat.EnableKeyword("_EMISSION");
            else
                mat.DisableKeyword("_EMISSION");
        }

        if (audioSource && switchSound)
            audioSource.PlayOneShot(switchSound);

        StopAllCoroutines();
        StartCoroutine(RotateLever(
            lightsOn ? leverOnRotation : leverOffRotation
        ));
    }

    IEnumerator RotateLever(Quaternion target)
    {
        isAnimating = true;

        while (Quaternion.Angle(lever.localRotation, target) > 0.1f)
        {
            lever.localRotation = Quaternion.Slerp(
                lever.localRotation,
                target,
                Time.deltaTime * leverSpeed
            );
            yield return null;
        }

        lever.localRotation = target;
        isAnimating = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            playerInside = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            playerInside = false;
    }
}