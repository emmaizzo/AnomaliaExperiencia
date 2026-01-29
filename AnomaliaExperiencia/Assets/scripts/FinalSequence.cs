using System.Collections;
using UnityEngine;
using Cinemachine;

public class IntroSequence : MonoBehaviour
{
    [Header("Camera")]
    public Transform camRig;
    public Transform camStart;
    public Transform camEnd;
    public float cameraDuration = 31f;

    [Header("Player")]
    public GameObject playerRoot;

    [Header("Fade")]
    public CanvasGroup fadeCanvas;
    public float fadeDuration = 1.5f;

    [Header("Audio")]
    public AudioSource introAudio;

    void Start()
    {
        playerRoot.SetActive(false);
        StartCoroutine(IntroRoutine());
    }

    IEnumerator IntroRoutine()
    {
        // Fade OUT
        yield return StartCoroutine(Fade(1, 0));

        camRig.position = camStart.position;
        camRig.rotation = camStart.rotation;

        introAudio.Play();

        float t = 0f;
        while (t < cameraDuration)
        {
            t += Time.deltaTime;
            float lerp = t / cameraDuration;

            camRig.position = Vector3.Lerp(
                camStart.position,
                camEnd.position,
                lerp
            );

            camRig.rotation = camStart.rotation;
            yield return null;
        }

        EndIntro();
    }

    void EndIntro()
    {
        playerRoot.SetActive(true);
        fadeCanvas.alpha = 0;
    }

    IEnumerator Fade(float from, float to)
    {
        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            fadeCanvas.alpha = Mathf.Lerp(from, to, t / fadeDuration);
            yield return null;
        }
        fadeCanvas.alpha = to;
    }
}