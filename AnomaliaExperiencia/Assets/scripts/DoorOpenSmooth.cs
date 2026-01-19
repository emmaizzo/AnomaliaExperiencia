using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorOpenSmooth : MonoBehaviour
{
    public float openAngle = 90f;
    public float speed = 2f;

    [Header("Audio")]
    public AudioSource sfxSource;
    public AudioSource bgSource;
    public AudioClip openSound;
    public AudioClip closeSound;
    public AudioClip introSound;
    public AudioClip lockedSound;

    [Header("Optional")]
    public DoorHandle handle;

    private Quaternion closedRotation;
    private Quaternion openRotation;

    private bool isOpen = false;
    private bool isLocked = true;

    private bool wasOpen = false;
    private bool closeSoundPlayed = false;

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
            StartCoroutine(PlayIntroWithDelay(0.5f));
        }
        else
        {
            UnlockDoor();
        }
    }

    void Update()
    {
        Quaternion target = isOpen ? openRotation : closedRotation;

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            target,
            Time.deltaTime * speed
        );

        // 🔊 sonido de cierre SOLO al terminar de cerrar
        if (!isOpen && wasOpen && !closeSoundPlayed)
        {
            if (Quaternion.Angle(transform.rotation, closedRotation) < 0.5f)
            {
                closeSoundPlayed = true;
                wasOpen = false;

                if (sfxSource != null && closeSound != null)
                    sfxSource.PlayOneShot(closeSound);
            }
        }
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
            if (sfxSource != null && lockedSound != null)
                sfxSource.PlayOneShot(lockedSound);
            return;
        }

        isOpen = true;
        wasOpen = true;
        closeSoundPlayed = false;

        if (sfxSource != null && openSound != null)
            sfxSource.PlayOneShot(openSound);

        // ✅ solo avisamos a la manija
        if (handle != null)
            handle.PressHandle();
    }

    public void CloseAndLockDoor()
    {
        if (!isOpen) return;

        isOpen = false;
        isLocked = true;
    }

    private IEnumerator PlayIntroWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        bgSource.clip = introSound;
        bgSource.Play();

        yield return new WaitForSeconds(introSound.length);
        UnlockDoor();
    }
}