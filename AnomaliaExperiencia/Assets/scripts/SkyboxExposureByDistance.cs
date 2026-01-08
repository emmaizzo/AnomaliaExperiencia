using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyboxExposureByDistance : MonoBehaviour
{
    public Transform player;

    public float exposureStart = 4.8f;
    public float exposureMin = -3f;

    public float distanceForFullDark = 50f;

    float startX;

    void Start()
    {
        startX = player.position.x;
        RenderSettings.skybox.SetFloat("_Exposure", exposureStart);
    }

    void Update()
    {
        float traveled = player.position.x - startX;
        float t = Mathf.Clamp01(traveled / distanceForFullDark);

        float exposure = Mathf.Lerp(exposureStart, exposureMin, t);
        RenderSettings.skybox.SetFloat("_Exposure", exposure);
    }
}

