using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorOpenSmooth : MonoBehaviour
{
    public float openAngle = 90f;
    public float speed = 2f;

    [Header("Audio")]
    public AudioSource sfxSource;      // Para abrir/cerrar
    public AudioSource bgSource;       // Para el audio de fondo (intro)
    public AudioClip openSound;
    public AudioClip closeSound;
    public AudioClip introSound;
    public AudioClip lockedSound;      // 🔒 sonido al intentar abrir puerta bloqueada

    [Header("Optional")]
    public DoorHandle handle;

    private Quaternion closedRotation;
    private Quaternion openRotation;

    private bool isOpen = false;
    private bool isLocked = true;

    void Start()
    {
        closedRotation = transform.rotation;
        openRotation = Quaternion.Euler(
            transform.eulerAngles.x,
            transform.eulerAngles.y + openAngle,
            transform.eulerAngles.z
        );

        if (introSound != null && bgSource != null)
        {
            StartCoroutine(PlayIntroWithDelay(2f)); // empieza 2s después
        }
        else
        {
            UnlockDoor();
        }
    }

    void Update()
    {
        Quaternion target = isOpen ? openRotation : closedRotation;
        transform.rotation = Quaternion.Slerp(transform.rotation, target, Time.deltaTime * speed);
    }

    void UnlockDoor()
    {
        isLocked = false;
    }

    public void OpenDoor()
    {
        if (isOpen) return;

        if (isLocked)
        {
            // 🔒 reproducir sonido de puerta bloqueada
            if (sfxSource != null && lockedSound != null)
                sfxSource.PlayOneShot(lockedSound);
            return;
        }

        isOpen = true;

        if (sfxSource != null && openSound != null)
            sfxSource.PlayOneShot(openSound);

        // animación de manija: presiona y vuelve
        if (handle != null)
            StartCoroutine(AnimateHandle());
    }

    public void CloseAndLockDoor()
    {
        if (!isOpen) return;

        isOpen = false;
        isLocked = true;

        if (sfxSource != null && closeSound != null)
            sfxSource.PlayOneShot(closeSound);

        if (handle != null)
            handle.CloseHandle(); // vuelve a posición inicial
    }

    private IEnumerator PlayIntroWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        bgSource.clip = introSound;
        bgSource.Play();

        yield return new WaitForSeconds(introSound.length);
        UnlockDoor();
    }

    // animación de la manija: presiona y vuelve automáticamente
    private IEnumerator AnimateHandle()
    {
        handle.PressHandle();                  // presiona
        yield return new WaitForSeconds(0.3f); // tiempo para el efecto
        handle.CloseHandle();                  // vuelve
    }
}