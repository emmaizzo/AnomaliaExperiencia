using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class InteractToChangeScene : MonoBehaviour
{
    public string sceneToLoad;
    public KeyCode interactKey = KeyCode.E;
    public float delayBeforeChange = 2f;

    bool playerInside = false;
    bool alreadyTriggered = false;

    void Update()
    {
        if (playerInside && !alreadyTriggered && Input.GetKeyDown(interactKey))
        {
            alreadyTriggered = true;
            StartCoroutine(ChangeSceneAfterDelay());
        }
    }

    IEnumerator ChangeSceneAfterDelay()
    {
        yield return new WaitForSeconds(delayBeforeChange);
        SceneManager.LoadScene(sceneToLoad);
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