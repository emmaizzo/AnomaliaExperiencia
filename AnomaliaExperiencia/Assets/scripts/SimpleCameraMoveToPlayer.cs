using System.Collections;
using UnityEngine;
using StarterAssets;
using UnityEngine.InputSystem;
using Cinemachine;

public class SimpleCameraMoveToPlayer : MonoBehaviour
{
    [Header("Camera")]
    public Transform camRig;
    public Transform camEnd;
    public float moveSpeed = 0.6f;

    public CinemachineVirtualCamera introCam;
    public CinemachineVirtualCamera playerCam;

    [Header("Player (ARRASTRAR DESDE INSPECTOR)")]
    public FirstPersonController playerController;
    public PlayerInput playerInput;
    public CharacterController characterController;

    AudioSource[] playerAudios;

    [Header("Fade")]
    public CanvasGroup fadeCanvas;
    public float fadeDuration = 2f;

    bool finished = false;

    void Awake()
    {
        if (fadeCanvas != null)
            fadeCanvas.alpha = 1f;

        // 🔒 bloqueo TOTAL
        if (playerController != null)
            playerController.enabled = false;

        if (playerInput != null)
            playerInput.enabled = false;

        if (characterController != null)
            characterController.enabled = false;

        // 🔇 muteamos audios del player
        if (characterController != null)
        {
            playerAudios = characterController.GetComponentsInChildren<AudioSource>();
            foreach (AudioSource a in playerAudios)
                a.mute = true;
        }

        // 🎥 cámaras
        if (introCam != null) introCam.Priority = 20;
        if (playerCam != null) playerCam.Priority = 0;
    }

    void Start()
    {
        if (fadeCanvas != null)
            StartCoroutine(Fade(1f, 0f));
    }

    void Update()
    {
        if (finished || camRig == null || camEnd == null) return;

        camRig.position = Vector3.MoveTowards(
            camRig.position,
            camEnd.position,
            moveSpeed * Time.deltaTime
        );

        if (Vector3.Distance(camRig.position, camEnd.position) < 0.01f)
        {
            finished = true;
            EndIntro();
        }
    }

    void EndIntro()
    {
        Debug.Log("FIN INTRO – devolviendo control");

        // 🎥 cambio de cámara
        if (introCam != null) introCam.Priority = 0;
        if (playerCam != null) playerCam.Priority = 20;

        // 🔊 devolver audios
        if (playerAudios != null)
        {
            foreach (AudioSource a in playerAudios)
                a.mute = false;
        }

        // 🔄 sincronizar rotación (evita giro raro)
        if (playerController != null && playerCam != null)
        {
            Vector3 euler = playerController.transform.eulerAngles;
            euler.y = playerCam.transform.eulerAngles.y;
            playerController.transform.eulerAngles = euler;
        }

        // 🎮 devolver control (orden correcto)
        if (characterController != null)
            characterController.enabled = true;

        if (playerInput != null)
            playerInput.enabled = true;

        if (playerController != null)
            playerController.enabled = true;
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