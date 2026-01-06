using UnityEngine;

public class EndlessCorridor : MonoBehaviour
{
    public Transform floorVisual;
    public Transform doorFront;
    public Transform doorBack;
    public Transform player;

    public float extendEvery = 5f;
    public float checkDistance = 3f;

    public float backDoorDistance = 2f;
    public float doorFollowSpeed = 3f;

    public BoxCollider[] colliders;

    float lastExtendX;
    float doorFrontTargetX;

    float lastPlayerX;   // <-- para saber si va adelante o atrás

    void Start()
    {
        lastExtendX = player.position.x;
        doorFrontTargetX = doorFront.position.x;

        lastPlayerX = player.position.x;
    }

    void Update()
    {
        // EXTENDER CORREDOR
        if (player.position.x - lastExtendX >= checkDistance)
        {
            Extend();
            lastExtendX = player.position.x;
        }

        // PUERTA ADELANTE
        Vector3 df = doorFront.position;
        df.x = Mathf.Lerp(doorFront.position.x, doorFrontTargetX, Time.deltaTime * 2f);
        doorFront.position = df;


        // ================================
        // **** PUERTA ATRÁS ****
        // ================================
        float movementX = player.position.x - lastPlayerX;
        lastPlayerX = player.position.x;

        // si movementX > 0 → jugador va adelante
        // si movementX < 0 → jugador va hacia atrás → NO mover puerta

        if (movementX > 0f)
        {
            Vector3 backTarget = doorBack.position;
            backTarget.x = player.position.x - backDoorDistance;

            doorBack.position = Vector3.Lerp
            (
                doorBack.position,
                backTarget,
                Time.deltaTime * doorFollowSpeed
            );
        }
    }

    void Extend()
    {
        // piso
        floorVisual.localScale += new Vector3(extendEvery, 0, 0);
        floorVisual.localPosition += new Vector3(extendEvery / 2f, 0, 0);

        // puerta delantera
        doorFrontTargetX += extendEvery;

        // colliders
        foreach (BoxCollider col in colliders)
        {
            Transform t = col.transform;

            t.localScale += new Vector3(extendEvery, 0, 0);
            t.localPosition += new Vector3(extendEvery / 2f, 0, 0);
        }

        Debug.Log("Piso + colliders extendidos");
    }
}
