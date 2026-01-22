using UnityEngine;

public class AnomalousSkyboxController : MonoBehaviour
{
    [Header("Skyboxes")]
    public Material[] anomalousSkyboxes;

    [Header("Reset Skybox")]
    public Material whiteSkybox;

    [Header("References")]
    public Transform player;
    public Transform soundSource;
    public AudioSource audioSource;
    public AudioClip snapSound;

    [Header("Timing")]
    public float startAfterSeconds = 20f;
    public float minSpeed = 6f;
    public float maxSpeed = 0.1f;

    [Header("Distance")]
    public float maxDistance = 60f;
    public float minDistance = 1.5f;
    public float exitDistance = 65f;

    [Header("Rotation")]
    public float minRotationSpeed = 0f;
    public float maxRotationSpeed = 80f;

    int currentIndex = 0;
    float timer = 0f;
    bool inAnomaly = false;

    void Update()
    {
        if (Time.time < startAfterSeconds)
            return;

        float distance = Vector3.Distance(player.position, soundSource.position);

        // 🔲 salir de la anomalía
        if (distance > exitDistance && inAnomaly)
        {
            ResetToWhite();
            return;
        }

        if (distance <= exitDistance)
            inAnomaly = true;

        float t = Mathf.InverseLerp(maxDistance, minDistance, distance);
        t = Mathf.Pow(t, 3f);

        float speed = Mathf.Lerp(minSpeed, maxSpeed, t);
        timer += Time.deltaTime * speed;

        float rotationSpeed = Mathf.Lerp(minRotationSpeed, maxRotationSpeed, t);
        RenderSettings.skybox.SetFloat(
            "_Rotation",
            RenderSettings.skybox.GetFloat("_Rotation") + rotationSpeed * Time.deltaTime
        );

        if (timer >= 1f)
        {
            timer = 0f;
            NextSkybox();
        }
    }

    void NextSkybox()
    {
        currentIndex = (currentIndex + 1) % anomalousSkyboxes.Length;
        RenderSettings.skybox = anomalousSkyboxes[currentIndex];
        DynamicGI.UpdateEnvironment();

        if (audioSource && snapSound)
            audioSource.PlayOneShot(snapSound);
    }

    void ResetToWhite()
    {
        inAnomaly = false;
        timer = 0f;

        RenderSettings.skybox = whiteSkybox;
        RenderSettings.skybox.SetFloat("_Rotation", 0f);
        DynamicGI.UpdateEnvironment();

        if (audioSource && snapSound)
            audioSource.PlayOneShot(snapSound);
    }
}