using UnityEngine;

public class EndlessCorridor : MonoBehaviour
{
    public Transform floorVisual;
    public Transform doorFront;
    public Transform doorBack;
    public Transform player;

    public float extendEvery = 5f;
    public float checkDistance = 3f;

    public float backDoorDistance = 2f;   // distancia detrás del jugador
    public float doorFollowSpeed = 3f;

    float lastExtendX;
    float doorFrontTargetX;

    void Start()
    {
        lastExtendX = player.position.x;
        doorFrontTargetX = doorFront.position.x;
    }

    void Update()
    {
        // ---- EXTENDER PASILLO ----
        if (player.position.x - lastExtendX >= checkDistance)
        {
            Extend();
            lastExtendX = player.position.x;
        }

        // ---- PUERTA DELANTERA (suave) ----
        Vector3 df = doorFront.position;
        df.x = Mathf.Lerp(doorFront.position.x, doorFrontTargetX, Time.deltaTime * 2f);
        doorFront.position = df;

        // ---- PUERTA TRASERA SIGUE AL PLAYER ----
        Vector3 backTarget = doorBack.position;
        backTarget.x = player.position.x - backDoorDistance;

        doorBack.position = Vector3.Lerp(
            doorBack.position,
            backTarget,
            Time.deltaTime * doorFollowSpeed
        );
    }

    void Extend()
    {
        floorVisual.localScale += new Vector3(extendEvery, 0, 0);
        floorVisual.localPosition += new Vector3(extendEvery / 2f, 0, 0);

        doorFrontTargetX += extendEvery;

        Debug.Log("PISO EXTENDIDO");
    }
}
