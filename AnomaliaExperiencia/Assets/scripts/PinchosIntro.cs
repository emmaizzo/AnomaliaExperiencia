using System.Collections;
using UnityEngine;

public class PinchosIntro : MonoBehaviour
{
    [Header("Black Screen")]
    public CanvasGroup blackScreen;
    public float blackDuration = 6f;
    public float fadeOutDuration = 1f;

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

        StartCoroutine(IntroSequence());
    }

    IEnumerator IntroSequence()
    {
        // audio dentro del negro
        yield return new WaitForSeconds(introAudioDelay);

        if (introAudio != null)
            introAudio.Play();

        // resto del tiempo negro
        yield return new WaitForSeconds(blackDuration - introAudioDelay);

        // fade out
        yield return StartCoroutine(FadeBlackScreen());

        // reactivar player
        player.SetActive(true);

        // audios finales
        if (musicAudio != null)
            musicAudio.Play();

        if (mainAudio != null)
            mainAudio.Play();
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
}