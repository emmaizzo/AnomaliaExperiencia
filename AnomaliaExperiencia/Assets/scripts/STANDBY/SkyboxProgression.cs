using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyboxProgression : MonoBehaviour
{
    public Transform player;
    public Material[] skyboxes;

    public float[] changeDistances = { 10f, 5f, 5f, 5f };

    public float darkenSpeed = 0.05f;
    public float rotationAcceleration = 5f;

    float nextChangeX;
    int index = 0;

    float currentExposure = 0f;
    float rotationSpeed = 0f;

    void Start()
    {
        nextChangeX = player.position.x + changeDistances[0];
        RenderSettings.skybox = skyboxes[0];
    }

    void Update()
    {
        // Progresión por avance
        if (index < skyboxes.Length - 1 && player.position.x >= nextChangeX)
        {
            index++;
            RenderSettings.skybox = skyboxes[index];

            if (index < changeDistances.Length)
                nextChangeX += changeDistances[index];

            DynamicGI.UpdateEnvironment();
        }

        // Último skybox: se rompe
        if (index == skyboxes.Length - 1)
        {
            // Oscurecer suavemente
            Color currentTint = RenderSettings.skybox.GetColor("_Tint");
            Color targetTint = Color.black;

            Color newTint = Color.Lerp(currentTint, targetTint, darkenSpeed * Time.deltaTime);
            RenderSettings.skybox.SetColor("_Tint", newTint);

            // Rotación cada vez más inestable
            rotationSpeed += rotationAcceleration * Time.deltaTime;
            RenderSettings.skybox.SetFloat(
                "_Rotation",
                RenderSettings.skybox.GetFloat("_Rotation") + rotationSpeed * Time.deltaTime
            );
        }

    }
}

