using UnityEngine;
using StarterAssets;

public class WaterPlayerPhysics : MonoBehaviour
{
    public FirstPersonController controller;

    [Header("Water feeling")]
    public float buoyancyForce = 3.5f;   // fuerza hacia arriba
    public float maxUpSpeed = 2f;

    CharacterController cc;

    bool isUnderwater;

    void Awake()
    {
        cc = GetComponent<CharacterController>();
    }

    public void SetUnderwater(bool value)
    {
        isUnderwater = value;
    }

    void Update()
    {
        if (!isUnderwater) return;

        if (cc == null) return;

        Vector3 vel = Vector3.up * buoyancyForce * Time.deltaTime;

        cc.Move(vel);

        // pequeño freno cuando cae
        if (controller != null)
        {
            // no tocamos variables internas
            // solo ayudamos a que no caiga tan rápido
        }
    }
}