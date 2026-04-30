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
        Vector3 dir = transform.position - hitPoint;
        rb.AddForceAtPosition(hitDirection * amount * 100f, hitPoint);

        Debug.Log($"ShootablePhysicsObject took {amount} damage at {hitPoint}");
    }
}
