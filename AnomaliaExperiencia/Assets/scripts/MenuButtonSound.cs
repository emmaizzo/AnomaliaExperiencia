using UnityEngine;

public class MenuButtonSound : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip clickSound;

    public void PlayClick()
    {
        if (audioSource && clickSound)
            audioSource.PlayOneShot(clickSound);
    }
}