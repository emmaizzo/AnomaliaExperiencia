using UnityEngine;

public class EyeLook : MonoBehaviour
{
    Transform player;
    bool follow = true;

    Quaternion baseRotation;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        baseRotation = transform.rotation; // 🔑 guarda cómo está acomodado
    }

    public void DisableLook()
    {
        follow = false;
    }

    void Update()
    {
        if (!follow || player == null) return;

        Vector3 dir = player.position - transform.position;

        Quaternion lookRot = Quaternion.LookRotation(dir);
        Quaternion finalRot = Quaternion.Slerp(
            baseRotation,
            lookRot,
            0.6f // cuánto puede girar
        );

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            finalRot,
            Time.deltaTime * 5f
        );
    }
}
