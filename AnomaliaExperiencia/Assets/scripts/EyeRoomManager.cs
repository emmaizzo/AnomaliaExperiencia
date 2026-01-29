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

    [Header("Botón")]
    public Transform botonTransform;
    public AudioSource sonidoBoton;
    public float distanciaBoton = 0.15f;
    public float duracionMovimientoBoton = 0.15f;

    [Header("Distribución")]
    public float distanciaMinimaEntreOjos = 0.7f;

    [Header("Tamaño de ojos")]
    public Vector2 rangoEscala = new Vector2(0.8f, 1.2f);

    [Header("Ojo Anómalo")]
    public Transform spawnOjoAnomalo;
    public float delayOjoAnomalo = 1f;
    public float radioExclusionAnomalo = 2.5f;

    [Header("Global Volume")]
    public Volume globalVolume;
    public float flashDuration = 0.5f;

    [Header("Audio Ambiente")]
    public AudioSource sonidoEvento;
    public AudioSource musicaAmbiente;
    public float tiempoAntesSonido = 10f;
    public float delayMusica = 1f;

    [Header("Música Dinámica")]
    public float incrementoPitch = 0.05f;
    public float pitchMaximo = 1.4f;
    public float velocidadCambioPitch = 0.6f;

    [Header("Panel Negro")]
    public CanvasGroup panelNegro;
    public float fadePanelDuration = 1.5f;

    // =============================
    // 🔊 AUDIOS AGREGADOS
    // =============================
    [Header("Audios Nuevos")]
    public AudioSource audioInicio;
    public float delayAudioInicio = 1f;

    public AudioSource audioCuartoClick;

    int contadorClicks = 0;
    bool audioCuartoClickReproducido = false;
    // =============================

    List<Vector3> posicionesUsadas = new List<Vector3>();

    bool ojoAnomaloSpawned = false;
    Vector3 posicionOjoAnomalo;

    bool botonEnMovimiento = false;
    Vector3 botonPosInicial;

    ColorAdjustments colorAdjust;
    Color originalColor;
    Coroutine pitchCoroutine;

    void Start()
    {
        if (panelNegro != null)
        {
            panelNegro.alpha = 1f;
            panelNegro.blocksRaycasts = true;
        }

        if (botonTransform != null)
            botonPosInicial = botonTransform.localPosition;

        if (globalVolume != null && globalVolume.profile.TryGet(out ColorAdjustments ca))
        {
            colorAdjust = ca;
            originalColor = colorAdjust.colorFilter.value;
        }

        StartCoroutine(RutinaInicial());
        StartCoroutine(RutinaPanelYAudio());

        // ▶️ AUDIO DE INICIO (AGREGADO)
        StartCoroutine(AudioInicio());
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
            CheckBoton();
    }

    // =============================
    // PANEL + AUDIO
    // =============================
    IEnumerator RutinaPanelYAudio()
    {
        yield return new WaitForSeconds(tiempoAntesSonido);

        yield return StartCoroutine(FadeCanvasGroup(panelNegro, 1f, 0f, fadePanelDuration));
        panelNegro.blocksRaycasts = false;

        if (sonidoEvento != null)
            sonidoEvento.Play();

        yield return new WaitForSeconds(delayMusica);

        if (musicaAmbiente != null)
            musicaAmbiente.Play();
    }

    IEnumerator FadeCanvasGroup(CanvasGroup cg, float from, float to, float duration)
    {
        float t = 0f;
        cg.alpha = from;

        while (t < 1f)
        {
            t += Time.deltaTime / duration;
            cg.alpha = Mathf.Lerp(from, to, t);
            yield return null;
        }

        cg.alpha = to;
    }

    // =============================
    // 🔊 AUDIO DE INICIO (AGREGADO)
    // =============================
    IEnumerator AudioInicio()
    {
        yield return new WaitForSeconds(delayAudioInicio);

        if (audioInicio != null)
            audioInicio.Play();
    }

    // =============================
    // BOTÓN
    // =============================
    void CheckBoton()
    {
        if (jugador == null || botonEnMovimiento) return;

        Ray ray = new Ray(jugador.position, jugador.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, distanciaInteraccion))
        {
            if (hit.collider.gameObject == objetoBoton)
            {
                SpawnOjos(ojosPorClick);

                if (colorAdjust != null)
                    StartCoroutine(FlashRed());

                if (sonidoBoton != null)
                    sonidoBoton.Play();

                AcelerarMusica();

                if (botonTransform != null)
                    StartCoroutine(MoverBoton());

                // 🔢 CONTADOR + AUDIO AL 5º CLICK (AGREGADO)
                contadorClicks++;

                if (contadorClicks >= 5 && !audioCuartoClickReproducido)
                {
                    audioCuartoClickReproducido = true;

                    if (audioCuartoClick != null)
                        audioCuartoClick.Play();
                }
            }
        }
    }

    void AcelerarMusica()
    {
        if (musicaAmbiente == null) return;

        float targetPitch = Mathf.Clamp(
            musicaAmbiente.pitch + incrementoPitch,
            1f,
            pitchMaximo
        );

        if (pitchCoroutine != null)
            StopCoroutine(pitchCoroutine);

        pitchCoroutine = StartCoroutine(LerpPitch(musicaAmbiente.pitch, targetPitch));
    }

    IEnumerator LerpPitch(float from, float to)
    {
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime * velocidadCambioPitch;
            musicaAmbiente.pitch = Mathf.Lerp(from, to, t);
            yield return null;
        }

        musicaAmbiente.pitch = to;
    }

    IEnumerator MoverBoton()
    {
        botonEnMovimiento = true;

        Vector3 abajo = botonPosInicial - Vector3.up * distanciaBoton;

        yield return StartCoroutine(MoverBotonLerp(botonPosInicial, abajo));
        yield return StartCoroutine(MoverBotonLerp(abajo, botonPosInicial));

        botonEnMovimiento = false;
    }

    IEnumerator MoverBotonLerp(Vector3 from, Vector3 to)
    {
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime / duracionMovimientoBoton;
            botonTransform.localPosition = Vector3.Lerp(from, to, t);
            yield return null;
        }

        botonTransform.localPosition = to;
    }

    // =============================
    // SPAWN
    // =============================
    IEnumerator RutinaInicial()
    {
        yield return new WaitForSeconds(esperaInicial);
        SpawnOjos(cantidadInicial);

        yield return new WaitForSeconds(delayOjoAnomalo);
        SpawnOjoAnomalo();
    }

    void SpawnOjos(int cantidad)
    {
        int spawneados = 0;
        int intentos = 0;

        Vector3 centroExclusion = spawnOjoAnomalo.position;

        while (spawneados < cantidad && intentos < cantidad * 50)
        {
            intentos++;

            Vector3 dir = GetRandomUpperHemisphereDirection();
            Vector3 pos = centroHabitacion.position + dir * radioSemiesfera;

            if (posicionesUsadas.Exists(p => Vector3.Distance(p, pos) < distanciaMinimaEntreOjos))
                continue;

            if (Vector3.Distance(pos, centroExclusion) < radioExclusionAnomalo)
                continue;

            posicionesUsadas.Add(pos);
            spawneados++;

            Vector3 normal = (pos - centroHabitacion.position).normalized;
            Quaternion rot = Quaternion.LookRotation(-normal) * Quaternion.Euler(0f, 180f, 0f);

            GameObject ojo = Instantiate(ojoNormalPrefab, pos, rot);
            ojo.transform.localScale = Vector3.one * Random.Range(rangoEscala.x, rangoEscala.y);

            EyeLookAtPlayer look = ojo.GetComponent<EyeLookAtPlayer>();
            if (look != null)
                look.SetTarget(jugador);
        }
    }

    void SpawnOjoAnomalo()
    {
        if (ojoAnomaloSpawned) return;

        ojoAnomaloSpawned = true;

        Quaternion rotacionFija = Quaternion.Euler(-39.111f, 43.769f, 38.189f);

        Instantiate(ojoAnomaloPrefab, spawnOjoAnomalo.position, rotacionFija);
    }

    IEnumerator FlashRed()
    {
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime / flashDuration;
            colorAdjust.colorFilter.value = Color.Lerp(originalColor, Color.red, t);
            yield return null;
        }

        t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / flashDuration;
            colorAdjust.colorFilter.value = Color.Lerp(Color.red, originalColor, t);
            yield return null;
        }
    }

    Vector3 GetRandomUpperHemisphereDirection()
    {
        float theta = Random.Range(0f, Mathf.PI * 2f);
        float phi = Random.Range(Mathf.Deg2Rad * 10f, Mathf.Deg2Rad * 85f);

        return new Vector3(
            Mathf.Sin(phi) * Mathf.Cos(theta),
            Mathf.Cos(phi),
            Mathf.Sin(phi) * Mathf.Sin(theta)
        );
    }
}