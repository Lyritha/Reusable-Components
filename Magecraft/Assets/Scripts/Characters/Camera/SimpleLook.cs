using Unity.Cinemachine;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class SimpleLook : Look
{
    protected Rigidbody rb;

    protected override void Awake()
    {
        base.Awake();
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate() => rb.MoveRotation(Quaternion.Euler(0f, Yaw, 0f));
}
