using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class LoadSceneButton : MonoBehaviour
{
    [Header("Scene")]
    public string sceneName;

    [Header("Black Screen Fade")]
    public CanvasGroup blackScreen;
    public float fadeDuration = 1f;

    public void LoadScene()
    {
        StartCoroutine(FadeAndLoad());
    }

    IEnumerator FadeAndLoad()
    {
        if (blackScreen != null)
        {
            blackScreen.blocksRaycasts = true;

            float t = 0f;
            while (t < fadeDuration)
            {
                t += Time.deltaTime;
                blackScreen.alpha = Mathf.Lerp(0f, 1f, t / fadeDuration);
                yield return null;
            }

            blackScreen.alpha = 1f;
        }

        SceneManager.LoadScene(sceneName);
    }
}