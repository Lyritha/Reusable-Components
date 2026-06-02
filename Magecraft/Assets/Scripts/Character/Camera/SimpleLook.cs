using Unity.Cinemachine;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class SimpleLook : Look
{
    [Header("Body Follow Settings"), SerializeField]
    protected Vector2 pitchLimits = new(-60f, 40f);

    protected Rigidbody rb;

    protected override void Awake()
    {
        base.Awake();
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate() => rb.MoveRotation(Quaternion.Euler(0f, yaw, 0f));
}
