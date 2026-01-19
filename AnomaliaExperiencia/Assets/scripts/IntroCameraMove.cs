using System.Collections;
using UnityEngine;
using StarterAssets;
using Cinemachine;
using UnityEngine.InputSystem;

public class IntroCameraMove : MonoBehaviour
{
    public CinemachineVirtualCamera introCam;

    [Header("Movimiento")]
    public float speed = 1.5f;
    public float duration = 6f;

    float timer = 0f;
    bool introFinished = false;

    float startX;
    float startY;

    FirstPersonController playerController;
    PlayerInput playerInput;

    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player == null)
        {
            Debug.LogError("No se encontró el Player con tag 'Player'");
            return;
        }

        playerController = player.GetComponent<FirstPersonController>();
        playerInput = player.GetComponent<PlayerInput>();

        if (playerController == null || playerInput == null)
        {
            Debug.LogError("Faltan componentes en el Player");
            return;
        }

        // Bloqueamos control del player
        playerController.enabled = false;
        playerInput.enabled = false;

        if (introCam != null)
        {
            introCam.Priority = 20;

            // 🔒 guardamos ejes iniciales
            startX = introCam.transform.position.x;
            startY = introCam.transform.position.y;
        }
    }

    void Update()
    {
        if (introFinished || introCam == null) return;

        if (timer < duration)
        {
            timer += Time.deltaTime;

            Vector3 forward = introCam.transform.forward;
            forward.y = 0f;
            forward.Normalize();

            Vector3 pos = introCam.transform.position;
            pos += forward * speed * Time.deltaTime;

            // 🔒 bloqueamos X e Y
            introCam.transform.position = new Vector3(
                startX,
                startY,
                pos.z
            );
        }
        else
        {
            EndIntro();
        }
    }

    void EndIntro()
    {
        introFinished = true;

        introCam.Priority = 0;

        playerController.enabled = true;
        playerInput.enabled = true;
    }
}