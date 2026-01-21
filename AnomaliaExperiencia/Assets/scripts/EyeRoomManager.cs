using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    List<Vector3> posicionesUsadas = new List<Vector3>();
    bool ojoAnomaloSpawned = false;

    void Start()
    {
        StartCoroutine(RutinaInicial());

        // por si este objeto tenía un mesh
        Renderer r = GetComponentInChildren<Renderer>();
        if (r != null) r.enabled = false;
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
                SpawnOjos(ojosPorClick);
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

            // ❌ muy cerca de otros ojos
            if (posicionesUsadas.Exists(p => Vector3.Distance(p, pos) < distanciaMinimaEntreOjos))
                continue;

            // ❌ muy cerca del ojo anómalo (zona fija)
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

        // 👉 ROTACIÓN FIJA
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