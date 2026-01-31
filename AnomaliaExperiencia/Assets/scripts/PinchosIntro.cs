using System.Collections;
using UnityEngine;

public class PinchosIntro : MonoBehaviour
{
    [Header("Black Screen")]
    public CanvasGroup blackScreen;
    public float blackDuration = 6f;
    public float fadeOutDuration = 1f;

    [Header("Title")]
    public CanvasGroup titleCanvas;
    public float titleFadeDuration = 0.5f;

    [Header("Audio")]
    public AudioSource introAudio;
    public AudioSource musicAudio;
    public AudioSource mainAudio;

    [Header("Timing")]
    public float introAudioDelay = 1f;

    GameObject player;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");

        // 🔒 BLOQUEO ABSOLUTO
        player.SetActive(false);

        blackScreen.alpha = 1f;

        if (titleCanvas != null)
            titleCanvas.alpha = 0f;

        StartCoroutine(IntroSequence());
        StartCoroutine(HandleTitle());
    }

    IEnumerator IntroSequence()
    {
        // audio dentro del negro
        yield return new WaitForSeconds(introAudioDelay);

        if (introAudio != null)
            introAudio.Play();

        // resto del tiempo negro
        yield return new WaitForSeconds(blackDuration - introAudioDelay);

        // fade out negro
        yield return StartCoroutine(FadeBlackScreen());

        // reactivar player
        player.SetActive(true);

        // audios finales
        if (musicAudio != null)
            musicAudio.Play();

        if (mainAudio != null)
            mainAudio.Play();
    }

    IEnumerator HandleTitle()
    {
        if (titleCanvas == null)
            yield break;

        // aparece 1s después de iniciar el negro
        yield return new WaitForSeconds(1f);
        yield return StartCoroutine(FadeCanvas(titleCanvas, 0f, 1f, titleFadeDuration));

        // visible hasta 1s antes del fade out del negro
        float visibleTime = blackDuration - 2f - titleFadeDuration;
        yield return new WaitForSeconds(Mathf.Max(0f, visibleTime));

        // fade out del título
        yield return StartCoroutine(FadeCanvas(titleCanvas, 1f, 0f, titleFadeDuration));
    }

    IEnumerator FadeBlackScreen()
    {
        float t = 0f;

        while (t < fadeOutDuration)
        {
            t += Time.deltaTime;
            blackScreen.alpha = Mathf.Lerp(1f, 0f, t / fadeOutDuration);
            yield return null;
        }

        blackScreen.alpha = 0f;
    }

    IEnumerator FadeCanvas(CanvasGroup canvas, float from, float to, float duration)
    {
        float t = 0f;
        canvas.alpha = from;

        while (t < duration)
        {
            t += Time.deltaTime;
            canvas.alpha = Mathf.Lerp(from, to, t / duration);
            yield return null;
        }

        canvas.alpha = to;
    }
}