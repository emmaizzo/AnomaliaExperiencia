using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class DoorHandleInteract : MonoBehaviour
{
    public string nextSceneName;
    public float waitBeforeLoad = 1f;

    bool playerInside = false;
    bool used = false;

    void Update()
    {
        if (playerInside && !used && Input.GetKeyDown(KeyCode.E))
        {
            used = true;
            StartCoroutine(LoadNextScene());
        }
    }

    IEnumerator LoadNextScene()
    {
        yield return StartCoroutine(ExitFade.Instance.FadeIn());
        yield return new WaitForSeconds(waitBeforeLoad);
        SceneManager.LoadScene(nextSceneName);
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