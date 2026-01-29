using System.Collections;
using UnityEngine;
using StarterAssets;
using UnityEngine.InputSystem;
using Cinemachine;

public class IntroCameraMove : MonoBehaviour
{
    [Header("Camera")]
    public Transform camRig;                  // rig de la intro (ya posicionado en escena)
    public Transform camEnd;                  // punto frente al player
    public float moveSpeed = 0.6f;

    public CinemachineVirtualCamera introCam;
    public CinemachineVirtualCamera playerCam;

    [Header("Player")]
    public GameObject playerRoot;

    FirstPersonController playerController;
    PlayerInput playerInput;
    CharacterController characterController;
    AudioSource[] playerAudios;

    [Header("Fade")]
    public CanvasGroup fadeCanvas;
    public float fadeDuration = 2f;

    [Header("Audio")]
    public AudioSource introAudio;

    bool finished = false;

    void Awake()
    {
        // negro inicial
        fadeCanvas.alpha = 1;

        // referencias player
        playerController = playerRoot.GetComponent<FirstPersonController>();
        playerInput = playerRoot.GetComponent<PlayerInput>();
        characterController = playerRoot.GetComponent<CharacterController>();

        // 🔒 bloqueo total
        playerController.enabled = false;
        playerInput.enabled = false;
        characterController.enabled = false;

        // 🔇 muteamos TODOS los audios del player (pasos incluidos)
        playerAudios = playerRoot.GetComponentsInChildren<AudioSource>();
        foreach (AudioSource a in playerAudios)
        {
            a.mute = true;
        }

        // prioridad de cámaras
        introCam.Priority = 20;
        playerCam.Priority = 0;
    }

    void Start()
    {
        // todo arranca junto
        introAudio.Play();
        StartCoroutine(Fade(1, 0));
    }

    void Update()
    {
        if (finished) return;

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
        // 🎥 cambio de cámara
        introCam.Priority = 0;
        playerCam.Priority = 20;

        // 🔊 devolvemos audios del player
        foreach (AudioSource a in playerAudios)
        {
            a.mute = false;
        }

        // 🎮 devolvemos control
        playerController.enabled = true;
        playerInput.enabled = true;
        characterController.enabled = true;
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