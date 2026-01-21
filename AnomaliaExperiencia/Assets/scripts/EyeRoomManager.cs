using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class EyeRoomManager : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject ojoNormalPrefab;
    public GameObject ojoAnomaloPrefab;

    [Header("Referencias")]
    public Transform jugador;
    public Transform centroHabitacion;

    [Header("Spawn inicial")]
    public float esperaInicial = 5f;
    public int cantidadInicial = 15;
    public float radioSemiesfera = 6f;

    [Header("Interacción")]
    public GameObject objetoBoton;
    public float distanciaInteraccion = 3f;
    public int ojosPorClick = 10;

    [Header("Distribución")]
    public float distanciaMinimaEntreOjos = 0.7f;

    [Header("Tamaño de ojos")]
    public Vector2 rangoEscala = new Vector2(0.8f, 1.2f);

    [Header("Ojo Anómalo")]
    public Transform spawnOjoAnomalo;
    public float delayOjoAnomalo = 1f;
    public float radioExclusionAnomalo = 2.5f;

    [Header("Ojo Base (solo referencia)")]
    public GameObject ojoBase;

    [Header("Global Volume")]
    public Volume globalVolume;
    public float flashDuration = 0.5f; // duración del flash rojo

    List<Vector3> posicionesUsadas = new List<Vector3>();
    bool ojoAnomaloSpawned = false;
    ColorAdjustments colorAdjust;
    Color originalColor;

    void Start()
    {
        // Ocultar mesh del ojo base
        if (ojoBase != null)
        {
            Renderer[] renders = ojoBase.GetComponentsInChildren<Renderer>();
            foreach (Renderer r in renders)
                r.enabled = false;
        }

        // Configurar Volume
        if (globalVolume != null && globalVolume.profile.TryGet(out ColorAdjustments ca))
        {
            colorAdjust = ca;
            originalColor = colorAdjust.colorFilter.value;
        }

        StartCoroutine(RutinaInicial());
    }

    IEnumerator RutinaInicial()
    {
        yield return new WaitForSeconds(esperaInicial);
        SpawnOjos(cantidadInicial);

        yield return new WaitForSeconds(delayOjoAnomalo);
        SpawnOjoAnomalo();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
            CheckBoton();
    }

    void CheckBoton()
    {
        if (jugador == null) return;

        Ray ray = new Ray(jugador.position, jugador.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, distanciaInteraccion))
        {
            if (hit.collider.gameObject == objetoBoton)
            {
                SpawnOjos(ojosPorClick);

                // Flash rojo del volume
                if (colorAdjust != null)
                    StartCoroutine(FlashRed());
            }
        }
    }

    // =============================
    // OJOS NORMALES (ESFERA)
    // =============================
    void SpawnOjos(int cantidad)
    {
        int spawneados = 0;
        int intentos = 0;
        int maxIntentos = cantidad * 30;

        while (spawneados < cantidad && intentos < maxIntentos)
        {
            intentos++;

            Vector3 dir = GetRandomUpperHemisphereDirection();
            Vector3 pos = centroHabitacion.position + dir * radioSemiesfera;

            if (posicionesUsadas.Exists(p => Vector3.Distance(p, pos) < distanciaMinimaEntreOjos))
                continue;

            if (ojoAnomaloSpawned &&
                Vector3.Distance(pos, spawnOjoAnomalo.position) < radioExclusionAnomalo)
                continue;

            posicionesUsadas.Add(pos);
            spawneados++;

            Vector3 normal = (pos - centroHabitacion.position).normalized;
            Quaternion rot = Quaternion.LookRotation(-normal) * Quaternion.Euler(0f, 180f, 0f);

            GameObject ojo = Instantiate(ojoNormalPrefab, pos, rot);
            float escala = Random.Range(rangoEscala.x, rangoEscala.y);
            ojo.transform.localScale = Vector3.one * escala;

            EyeLookAtPlayer look = ojo.GetComponent<EyeLookAtPlayer>();
            if (look != null)
                look.SetTarget(jugador);
        }

        if (spawneados < cantidad)
            Debug.Log($"🟡 No entraron todos los ojos ({spawneados}/{cantidad})");
    }

    // =============================
    // OJO ANÓMALO (FIJO)
    // =============================
    void SpawnOjoAnomalo()
    {
        if (ojoAnomaloSpawned) return;

        if (ojoAnomaloPrefab == null || spawnOjoAnomalo == null)
        {
            Debug.LogError("❌ Falta prefab o spawn del ojo anómalo");
            return;
        }

        ojoAnomaloSpawned = true;

        // Rotación fija
        Quaternion rot = Quaternion.Euler(-50.962f, 60.03f, 14.597f);

        GameObject ojo = Instantiate(
            ojoAnomaloPrefab,
            spawnOjoAnomalo.position,
            rot
        );

        EyeLookAtPlayer look = ojo.GetComponent<EyeLookAtPlayer>();
        if (look != null)
            look.SetTarget(jugador);

        Debug.Log("🧿 Ojo anómalo spawneado con rotación fija");
    }

    // =============================
    // Animación flash rojo
    // =============================
    private IEnumerator FlashRed()
    {
        float t = 0;
        Color targetColor = Color.red;

        // Ir hacia rojo
        while (t < 1)
        {
            t += Time.deltaTime / flashDuration;
            colorAdjust.colorFilter.value = Color.Lerp(originalColor, targetColor, t);
            yield return null;
        }

        // Volver a original
        t = 0;
        while (t < 1)
        {
            t += Time.deltaTime / flashDuration;
            colorAdjust.colorFilter.value = Color.Lerp(targetColor, originalColor, t);
            yield return null;
        }
    }

    // =============================
    Vector3 GetRandomUpperHemisphereDirection()
    {
        float theta = Random.Range(0f, Mathf.PI * 2f);
        float phi = Random.Range(Mathf.Deg2Rad * 10f, Mathf.Deg2Rad * 85f);

        float x = Mathf.Sin(phi) * Mathf.Cos(theta);
        float y = Mathf.Cos(phi);
        float z = Mathf.Sin(phi) * Mathf.Sin(theta);

        return new Vector3(x, y, z);
    }
}