using UnityEngine;
using UnityEngine.SceneManagement;

public class StressManager : MonoBehaviour
{
    public static StressManager Instance;

    public int stressLevel;
    public float speedMultiplier = 1f;
    public float speedIncreasePerTrigger = 0.3f;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void IncreaseStress()
    {
        stressLevel++;
        speedMultiplier += speedIncreasePerTrigger;
    }

    public void ResetStress()
    {
        stressLevel = 0;
        speedMultiplier = 1f;
    }
}