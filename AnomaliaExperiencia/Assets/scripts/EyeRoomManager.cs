using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EyeRoomManager : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject ojoNormalPrefab;
    public GameObject ojoAnomaloPrefab;

    [Header("Referencias")]
    public Transform jugador;          // Main Camera
    public Transform centroHabitacion; // Centro de la esfera

    [Header("Spawn inicial")]
    public float esperaInicial = 5f;
    public int cantidadInicial = 15;
    public float radioSemiesfera = 6f;

    [Header("Interacción con botón")]
    public GameObject objetoBoton;
    public float distanciaInteraccion = 3f;
    public int ojosPorClick = 10;

    [Header("Distribución")]
    public float distanciaMinimaEntreOjos = 0.7f;

    List<Vector3> posicionesUsadas = new List<Vector3>();

    void Start()
    {
        StartCoroutine(RutinaInicial());
    }

    IEnumerator RutinaInicial()
    {
        yield return new WaitForSeconds(esperaInicial);
        SpawnOjos(cantidadInicial, false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
            CheckBoton();
        Debug.Log(transform.position);
    }

    void CheckBoton()
    {
        Ray ray = new Ray(jugador.position, jugador.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, distanciaInteraccion))
        {
            if (hit.collider.gameObject == objetoBoton)
            {
                SpawnOjos(ojosPorClick, true);
            }
        }
    }

    void SpawnOjos(int cantidad, bool incluirAnomalia)
    {
        for (int i = 0; i < cantidad; i++)
        {
            Vector3 pos;
            int intentos = 0;

            do
            {
                Vector3 dir = GetRandomUpperHemisphereDirection();
                pos = centroHabitacion.position + dir * radioSemiesfera;
                intentos++;
            }
            while (
                posicionesUsadas.Exists(p => Vector3.Distance(p, pos) < distanciaMinimaEntreOjos)
                && intentos < 30
            );

            posicionesUsadas.Add(pos);

            // normal desde el centro
            Vector3 normal = (pos - centroHabitacion.position).normalized;

            // el ojo sale de la esfera pero mira hacia adentro
            Quaternion rot = Quaternion.LookRotation(-normal);

            // ajuste por orientación del modelo
            rot *= Quaternion.Euler(0f, 180f, 0f);

            GameObject ojo = Instantiate(ojoNormalPrefab, pos, rot);

            EyeLookAtPlayer look = ojo.GetComponent<EyeLookAtPlayer>();
            if (look != null)
                look.SetTarget(jugador);
        }

        if (incluirAnomalia && ojoAnomaloPrefab != null)
        {
            Vector3 dir = GetRandomUpperHemisphereDirection();
            Vector3 pos = centroHabitacion.position + dir * radioSemiesfera;

            Vector3 normal = (pos - centroHabitacion.position).normalized;
            Quaternion rot = Quaternion.LookRotation(-normal) * Quaternion.Euler(0f, 180f, 0f);

            Instantiate(ojoAnomaloPrefab, pos, rot);
        }
    }

    Vector3 GetRandomUpperHemisphereDirection()
    {
        float theta = Random.Range(0f, Mathf.PI * 2f);
        float phi = Random.Range(
        Mathf.Deg2Rad * 30f,
        Mathf.Deg2Rad * 75f
        );

        float x = Mathf.Sin(phi) * Mathf.Cos(theta);
        float y = Mathf.Cos(phi);
        float z = Mathf.Sin(phi) * Mathf.Sin(theta);

        return new Vector3(x, y, z);
    }
}