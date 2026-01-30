using System.Collections;
using UnityEngine;

public class RisingDoorTrigger : MonoBehaviour
{
    [Header("Door")]
    public Transform door;
    public float riseHeight = 2.5f;
    public float riseDuration = 3f;

    [Header("Audio")]
    public AudioSource riseAudio;

    bool activated = false;
    Vector3 startPos;

    void Start()
    {
        if (door != null)
            startPos = door.position;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (activated)
            return;

        if (other.CompareTag("Player"))
        {
            activated = true;
            StartCoroutine(RiseDoor());
        }
    }

    IEnumerator RiseDoor()
    {
        if (riseAudio != null)
            riseAudio.Play();

        Vector3 endPos = startPos + Vector3.up * riseHeight;

        float t = 0f;

        while (t < riseDuration)
        {
            t += Time.deltaTime;

            float k = t / riseDuration;
            door.position = Vector3.Lerp(startPos, endPos, k);

            yield return null;
        }

        door.position = endPos;
    }
}