using UnityEngine;

public class DoorSkyboxTrigger : MonoBehaviour
{
    [Header("References")]
    public AnomalousSkyboxController anomalousSkybox;
    public Material whiteSkybox;

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        if (anomalousSkybox != null)
            anomalousSkybox.enabled = false;

        if (whiteSkybox != null)
        {
            RenderSettings.skybox = whiteSkybox;

            // 🔽 CLAVE: bajar iluminación ambiental
            RenderSettings.ambientIntensity = 0.6f;
            RenderSettings.ambientLight = Color.white;

            RenderSettings.skybox.SetFloat("_Rotation", 0f);
            DynamicGI.UpdateEnvironment();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        // ✅ vuelve la anomalía
        if (anomalousSkybox != null)
            anomalousSkybox.enabled = true;
    }
}