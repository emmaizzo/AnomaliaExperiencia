using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetManager : MonoBehaviour
{
    public static ResetManager Instance;

    [Header("Fade")]
    public CanvasGroup fadeCanvas;
    public float fadeDuration = 1f;

    [Header("Player")]
    public Transform player;
    public Transform playerSpawn;

    CharacterController cc;
    MonoBehaviour[] playerScripts;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        cc = player.GetComponent<CharacterController>();
        playerScripts = player.GetComponents<MonoBehaviour>();
    }

    public void ResetRoom()
    {
        StartCoroutine(ResetRoutine());
    }

    IEnumerator ResetRoutine()
    {
        // Fade to black
        yield return StartCoroutine(Fade(1f));

        // Reset stress
        StressManager.Instance.ResetStress();

        TrapdoorTrigger trap = FindObjectOfType<TrapdoorTrigger>();
        if (trap != null)
            trap.ResetVolumeVisuals();

        // Reset all resettable objects
        ResettableObject[] resettable = FindObjectsOfType<ResettableObject>();
        foreach (var obj in resettable)
            obj.ResetState();

        yield return null; // dejar que Unity procese el reset

        // -------- RESET PLAYER (FORMA SEGURA) --------

        // Apagar scripts del player (movimiento, cámara, etc)
        foreach (var s in playerScripts)
        {
            if (!(s is CharacterController))
                s.enabled = false;
        }

        yield return null;

        // Apagar CC
        cc.enabled = false;

        // Resetear posición
        player.position = playerSpawn.position;
        player.rotation = playerSpawn.rotation;

        yield return null;

        // Volver a prender CC
        cc.enabled = true;

        // Volver a prender scripts
        foreach (var s in playerScripts)
        {
            s.enabled = true;
        }

        // --------------------------------------------

        yield return new WaitForSeconds(0.05f);

        // Fade back
        yield return StartCoroutine(Fade(0f));
    }

    public IEnumerator Fade(float target)
    {
        float start = fadeCanvas.alpha;
        float t = 0f;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            fadeCanvas.alpha = Mathf.Lerp(start, target, t / fadeDuration);
            yield return null;
        }

        fadeCanvas.alpha = target;
    }
}