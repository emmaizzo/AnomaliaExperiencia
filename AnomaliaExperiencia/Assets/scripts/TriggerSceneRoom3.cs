using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TriggerSceneRoom3 : MonoBehaviour
{
    [Header("Scene")]
    public string nextSceneName;

    [Header("Interaction")]
    public KeyCode interactKey = KeyCode.E;

    [Header("Black Screen")]
    public CanvasGroup blackScreen;
    public float fadeInDuration = 2f;
    public float blackScreenDuration = 12f;

    [Header("Black screen audio (plays after delay)")]
    public AudioSource blackScreenAudio;
    public float blackScreenAudioDelay = 1f;

    [Header("Audios to fade out")]
    public List<AudioSource> audiosToFadeOut = new List<AudioSource>();
    public float audioFadeSpeed = 1.5f;

    [Header("Footsteps (must NEVER sound)")]
    public AudioSource footstepAudio;

    bool playerInside;
    bool used;

    AudioSource[] allAudioSources;

    void Start()
    {
        if (blackScreen != null)
        {
            blackScreen.alpha = 0f;
            blackScreen.blocksRaycasts = false;
        }
    }

    void Update()
    {
        if (!playerInside || used)
            return;

        if (Input.GetKeyDown(interactKey))
        {
            used = true;
            StartCoroutine(Sequence());
        }
    }

    IEnumerator Sequence()
    {
        // 🔎 agarramos todos los audios activos
        allAudioSources = FindObjectsOfType<AudioSource>();

        // 🚫 PASOS: existen pero no pueden sonar
        if (footstepAudio != null)
        {
            footstepAudio.Stop();
            footstepAudio.mute = true;
            footstepAudio.enabled = false;
        }

        // 🔉 fade out de audios específicos
        foreach (var a in audiosToFadeOut)
        {
            if (a != null)
                StartCoroutine(FadeOutAudio(a));
        }

        // 🔇 apagar absolutamente todo menos el blackScreenAudio
        foreach (var a in allAudioSources)
        {
            if (a == null)
                continue;

            if (a == blackScreenAudio)
                continue;

            a.Stop();
        }

        // 🖤 fade in del black screen
        if (blackScreen != null)
        {
            blackScreen.blocksRaycasts = true;

            float t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime / fadeInDuration;
                blackScreen.alpha = Mathf.Clamp01(t);
                yield return null;
            }

            blackScreen.alpha = 1f;
        }

        // ⏱ delay antes del audio
        yield return new WaitForSeconds(blackScreenAudioDelay);

        // 🔊 único audio permitido
        if (blackScreenAudio != null)
            blackScreenAudio.Play();

        yield return new WaitForSeconds(blackScreenDuration - blackScreenAudioDelay);

        SceneManager.LoadScene(nextSceneName);
    }

    IEnumerator FadeOutAudio(AudioSource source)
    {
        if (source == null)
            yield break;

        float startVolume = source.volume;

        while (source.volume > 0f)
        {
            source.volume = Mathf.MoveTowards(
                source.volume,
                0f,
                Time.deltaTime * audioFadeSpeed
            );
            yield return null;
        }

        source.Stop();
        source.volume = startVolume;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            playerInside = true;
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            playerInside = false;
    }
}