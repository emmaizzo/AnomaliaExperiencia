using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class InteractToChangeScene : MonoBehaviour
{
    public string sceneToLoad;
    public KeyCode interactKey = KeyCode.E;

    bool playerInside = false;
    bool alreadyTriggered = false;

    // ----------------------------
    // BLACK SCREEN
    // ----------------------------
    [Header("Black Screen")]
    public CanvasGroup blackScreen;
    public float blackScreenDuration = 12f;
    public AudioSource blackScreenAudio; // suena al segundo 1

    // ----------------------------
    // Fade Out Other Audios
    // ----------------------------
    [Header("Fade Out Other Audios")]
    public List<AudioSource> audiosToFadeOut = new List<AudioSource>();
    public float fadeOutSpeed = 1.5f;

    // ----------------------------
    // NUEVO – referencia a la habitación
    // ----------------------------
    [Header("Room Controller")]
    public RisingWater risingWater;

    void Update()
    {
        if (playerInside && !alreadyTriggered && Input.GetKeyDown(interactKey))
        {
            alreadyTriggered = true;
            StartCoroutine(BlackScreenAndChangeScene());
        }
    }

    IEnumerator BlackScreenAndChangeScene()
    {
        // 👉 corta toda la lógica de audio de la habitación
        if (risingWater != null)
            risingWater.StopAllRoomAudio();

        // fade de audios externos (player, ambiente general, etc)
        foreach (var a in audiosToFadeOut)
        {
            if (a != null)
                StartCoroutine(FadeOutAudio(a));
        }

        if (blackScreen != null)
        {
            blackScreen.alpha = 1f;
            blackScreen.blocksRaycasts = true;
        }

        yield return new WaitForSeconds(1f);

        blackScreenAudio?.Play();

        yield return new WaitForSeconds(blackScreenDuration - 1f);

        SceneManager.LoadScene(sceneToLoad);
    }

    IEnumerator FadeOutAudio(AudioSource source)
    {
        if (source == null) yield break;

        while (source.volume > 0f)
        {
            source.volume = Mathf.MoveTowards(
                source.volume,
                0f,
                Time.deltaTime * fadeOutSpeed
            );

            yield return null;
        }

        source.Stop();
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