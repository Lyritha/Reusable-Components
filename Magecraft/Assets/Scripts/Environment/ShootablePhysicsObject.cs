using UnityEngine;

public class ShootablePhysicsObject : MonoBehaviour, IDamageable
{
    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void TakeDamage(int amount, Vector3 hitPoint, Vector3 hitDirection)
    {
        rb.AddForceAtPosition(100f * amount * hitDirection, hitPoint);
    }
}
