using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EyesRoomManager : MonoBehaviour
{
    public Transform player;
    public GameObject eyePrefab;
    public AudioSource lightSound;

    public int initialEyes = 25;
    public int extraEyesOnButton = 20;
    public float radius = 10f;
    public float minEyeScale = 0.8f;
    public float maxEyeScale = 1.15f;

    private List<GameObject> eyes = new List<GameObject>();

    void Start()
    {
        StartCoroutine(SpawnEyesDelayed());
    }

    IEnumerator SpawnEyesDelayed()
    {
        yield return new WaitForSeconds(1f);
        SpawnEyes(initialEyes);
        lightSound?.Play();
    }

    void SpawnEyes(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            float scale = Random.Range(minEyeScale, maxEyeScale);
            float minDistance = scale * 2.2f;

            Vector3 worldPos;
            int tries = 0;
            bool valid = false;

            do
            {
                float theta = Random.Range(0f, Mathf.PI * 2f);
                float phi = Random.Range(0.2f, Mathf.PI / 2f);

                Vector3 dir = new Vector3(
                    Mathf.Sin(phi) * Mathf.Cos(theta),
                    Mathf.Cos(phi),
                    Mathf.Sin(phi) * Mathf.Sin(theta)
                );

                worldPos = player.position + dir * radius;

                if (!TooClose(worldPos, minDistance))
                    valid = true;

                tries++;
            }
            while (!valid && tries < 40);

            if (!valid) continue;

            GameObject eye = Instantiate(
                eyePrefab,
                worldPos,
                eyePrefab.transform.rotation // 🔑 CLAVE
            );

            eye.transform.localScale *= scale;
            eyes.Add(eye);
        }
    }

    bool TooClose(Vector3 pos, float minDist)
    {
        foreach (var e in eyes)
        {
            if (Vector3.Distance(e.transform.position, pos) < minDist)
                return true;
        }
        return false;
    }

    public void SpawnMoreEyes()
    {
        SpawnEyes(extraEyesOnButton);
    }
}
