using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitFade : MonoBehaviour
{
    public static ExitFade Instance;

    public CanvasGroup canvasGroup;
    public float fadeDuration = 1f;

    void Awake()
    {
        Instance = this;
    }

    public IEnumerator FadeIn()
    {
        yield return StartCoroutine(Fade(1f));
    }

    IEnumerator Fade(float target)
    {
        float start = canvasGroup.alpha;
        float t = 0f;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(start, target, t / fadeDuration);
            yield return null;
        }

        canvasGroup.alpha = target;
    }
}