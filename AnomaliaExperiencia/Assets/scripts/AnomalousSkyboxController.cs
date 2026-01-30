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

    [Header("Near sound")]
    public AudioClip nearSound;
    public float nearSoundDistance = 3f;

    [Header("Timing")]
    public float startAfterSeconds = 20f;
    public float minSpeed = 6f;
    public float maxSpeed = 0.1f;

    [Header("Distance")]
    public float maxDistance = 60f;
    public float minDistance = 1.5f;

    [Header("Rotation")]
    public float minRotationSpeed = 0f;
    public float maxRotationSpeed = 80f;

    AudioSource footstepAudio;   // 👈 encontrado automáticamente

    int currentIndex = 0;
    float timer = 0f;
    bool nearSoundPlayed = false;
    bool footstepsEnabled = false;

    void Start()
    {
        // 🔎 buscamos el AudioSource de pasos en el player o hijos
        footstepAudio = player.GetComponentInChildren<AudioSource>();

        // 🔇 apagamos pasos al inicio (skybox negro)
        if (footstepAudio != null)
            footstepAudio.mute = true;
    }

    void Update()
    {
        if (Time.time < startAfterSeconds)
            return;

        // 🔊 activamos pasos UNA SOLA VEZ
        if (!footstepsEnabled)
        {
            footstepsEnabled = true;

            if (footstepAudio != null)
                footstepAudio.mute = false;
        }

        float distance = Vector3.Distance(player.position, soundSource.position);

        if (!nearSoundPlayed && distance <= nearSoundDistance)
        {
            nearSoundPlayed = true;

            if (audioSource != null && nearSound != null)
                audioSource.PlayOneShot(nearSound);
        }

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

        if (audioSource != null && snapSound != null)
            audioSource.PlayOneShot(snapSound);
    }
}