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
    public float blackScreenDuration = 12f;

    [Header("Black screen audio (plays after 1s)")]
    public AudioSource blackScreenAudio;
    public float blackScreenAudioDelay = 1f;

    [Header("Audios to fade out")]
    public List<AudioSource> audiosToFadeOut = new List<AudioSource>();
    public float audioFadeSpeed = 1.5f;

    [Header("Sound when pressing E")]
    public AudioSource pressAudio;

    [Header("Footsteps")]
    public AudioSource footstepAudio;   // 👈 PASOS

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

            if (pressAudio != null)
                pressAudio.Play();

            StartCoroutine(Sequence());
        }
    }

    IEnumerator Sequence()
    {
        allAudioSources = FindObjectsOfType<AudioSource>();

        // 🔇 apagar pasos
        if (footstepAudio != null)
            footstepAudio.mute = true;

        // Fade out definidos
        foreach (var a in audiosToFadeOut)
        {
            if (a != null)
                StartCoroutine(FadeOutAudio(a));
        }

        // Pausar el resto
        foreach (var a in allAudioSources)
        {
            if (a == null)
                continue;

            if (a == blackScreenAudio || a == pressAudio)
                continue;

            if (audiosToFadeOut.Contains(a))
                continue;

            a.Pause();
        }

        // Black screen ON
        if (blackScreen != null)
        {
            blackScreen.alpha = 1f;
            blackScreen.blocksRaycasts = true;
        }

        yield return new WaitForSeconds(blackScreenAudioDelay);

        if (blackScreenAudio != null)
            blackScreenAudio.Play();

        yield return new WaitForSeconds(blackScreenDuration - blackScreenAudioDelay);

        SceneManager.LoadScene(nextSceneName);
    }

    IEnumerator FadeOutAudio(AudioSource source)
    {
        if (source == null)
            yield break;

        float start = source.volume;

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
        source.volume = start;
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