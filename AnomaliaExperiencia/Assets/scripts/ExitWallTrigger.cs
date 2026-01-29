using System.Collections;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine;

public class ExitWallTrigger : MonoBehaviour
{
    public Transform player;
    public Transform nextRoomSpawn;

    [Header("Objects to hide")]
    public GameObject[] spikesToDisable;

    [Header("Skybox")]
    public Material newSkybox;

    [Header("Flashlight")]
    public Light flashlight;
    public float flashlightFadeDuration = 0.5f;

    [Header("Room Lights")]
    public Light[] roomLights;
    public float roomLightIntensity = 1f;

    [Header("Global Volume")]
    public Volume globalVolume;
    public float whiteRoomTransitionTime = 1f;

    [Header("Music")]
    public AudioSource firstMusic;
    public AudioSource roomMusic;
    public float musicFadeOutTime = 1f;
    public float musicFadeInTime = 1f;

    [Header("Extra Audio")]
    public AudioSource extraAudioToStop;

    [Header("Enter Audio")]
    public AudioSource enterAudio;
    public float enterAudioDelay = 1f;

    Vignette vignette;
    ColorAdjustments color;
    DepthOfField dof;

    Collider wallCollider;
    bool used = false;

    void Awake()
    {
        wallCollider = GetComponent<Collider>();

        foreach (var l in roomLights)
            if (l != null) l.enabled = false;

        if (globalVolume != null && globalVolume.profile != null)
        {
            globalVolume.profile.TryGet(out vignette);
            globalVolume.profile.TryGet(out color);
            globalVolume.profile.TryGet(out dof);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (!used && other.CompareTag("Player"))
        {
            used = true;

            // 🔊 audio con delay
            if (enterAudio != null)
                StartCoroutine(PlayEnterAudio());

            StartCoroutine(Transition());
        }
    }

    IEnumerator PlayEnterAudio()
    {
        yield return new WaitForSeconds(enterAudioDelay);
        enterAudio.Play();
    }

    IEnumerator Transition()
    {
        if (firstMusic != null)
            StartCoroutine(FadeOutMusic(firstMusic));

        if (extraAudioToStop != null)
            StartCoroutine(FadeOutMusic(extraAudioToStop));

        foreach (var s in spikesToDisable)
            if (s != null) s.SetActive(false);

        if (flashlight != null)
            yield return StartCoroutine(FadeFlashlight());

        if (newSkybox != null)
        {
            RenderSettings.skybox = newSkybox;
            DynamicGI.UpdateEnvironment();
        }

        yield return null;

        CharacterController cc = player.GetComponent<CharacterController>();
        cc.enabled = false;
        player.position = nextRoomSpawn.position;
        player.rotation = nextRoomSpawn.rotation;
        cc.enabled = true;

        if (roomMusic != null)
            StartCoroutine(FadeInMusic(roomMusic));

        foreach (var l in roomLights)
        {
            if (l != null)
            {
                l.enabled = true;
                l.intensity = roomLightIntensity;
            }
        }

        StartCoroutine(WhiteRoomVolume());

        wallCollider.isTrigger = false;
        enabled = false;
    }

    IEnumerator WhiteRoomVolume()
    {
        float startWeight = globalVolume.weight;
        float startVignette = vignette.intensity.value;
        float startExposure = color.postExposure.value;
        float startSaturation = color.saturation.value;
        float startContrast = color.contrast.value;
        float startBlur = dof.gaussianEnd.value;

        float t = 0f;

        while (t < whiteRoomTransitionTime)
        {
            t += Time.deltaTime;
            float lerp = t / whiteRoomTransitionTime;

            globalVolume.weight = Mathf.Lerp(startWeight, 1f, lerp);
            vignette.intensity.value = Mathf.Lerp(startVignette, 0.05f, lerp);
            color.postExposure.value = Mathf.Lerp(startExposure, 0.2f, lerp);
            color.saturation.value = Mathf.Lerp(startSaturation, -40f, lerp);
            color.contrast.value = Mathf.Lerp(startContrast, -10f, lerp);
            dof.gaussianEnd.value = Mathf.Lerp(startBlur, 3f, lerp);

            yield return null;
        }
    }

    IEnumerator FadeFlashlight()
    {
        float startIntensity = flashlight.intensity;
        float t = 0f;

        while (t < flashlightFadeDuration)
        {
            t += Time.deltaTime;
            flashlight.intensity = Mathf.Lerp(startIntensity, 0f, t / flashlightFadeDuration);
            yield return null;
        }

        flashlight.intensity = 0f;
    }

    IEnumerator FadeOutMusic(AudioSource music)
    {
        float startVolume = music.volume;
        float t = 0f;

        while (t < musicFadeOutTime)
        {
            t += Time.deltaTime;
            music.volume = Mathf.Lerp(startVolume, 0f, t / musicFadeOutTime);
            yield return null;
        }

        music.volume = 0f;
        music.Stop();
    }

    IEnumerator FadeInMusic(AudioSource music)
    {
        music.volume = 0f;
        music.Play();

        float t = 0f;

        while (t < musicFadeInTime)
        {
            t += Time.deltaTime;
            music.volume = Mathf.Lerp(0f, 1f, t / musicFadeInTime);
            yield return null;
        }

        music.volume = 1f;
    }
}