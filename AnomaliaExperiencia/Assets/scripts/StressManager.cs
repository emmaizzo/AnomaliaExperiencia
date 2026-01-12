using UnityEngine;
using UnityEngine.SceneManagement;

public class StressManager : MonoBehaviour
{
    public static StressManager Instance;

    [Header("Stress")]
    public int stressLevel = 0;

    [Header("Wall Speed")]
    public float baseWallSpeed = 0.2f;
    public float speedPerStress = 0.15f;

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
    }

    public float GetWallSpeed()
    {
        return baseWallSpeed + stressLevel * speedPerStress;
    }

    public void ResetScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}