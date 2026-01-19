using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class DoorTransition : MonoBehaviour
{
    [Header("Referencias")]
    public FadeController fadeController;
    public AudioFadeController audioFade;
    public AudioSource doorSound;

    [Header("Escena")]
    public string nextSceneName;

    bool playerInside = false;
    bool triggered = false;

    void Update()
    {
        if (playerInside && !triggered && Input.GetKeyDown(KeyCode.E))
        {
            triggered = true;

            if (doorSound != null)
                doorSound.Play();

            if (audioFade != null)
                audioFade.FadeOutAll();

            if (fadeController != null)
            {
                fadeController.FadeToBlack(() =>
                {
                    SceneManager.LoadScene(nextSceneName);
                });
            }
        }
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