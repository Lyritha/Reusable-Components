using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CharacterJumpSingle : MonoBehaviour, ICharacterJump
{
    [SerializeField]
    private float jumpImpulse = 180f;

    private Rigidbody rb;
    private Collider col;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
    }

    public void OnJump()
    {
        if (IsGrounded())
        {
            // Remove downward velocity so jump is consistent
            Vector3 vel = rb.linearVelocity;
            if (vel.y < 0) vel.y = 0;
            rb.linearVelocity = vel;

            rb.AddForce(Vector3.up * jumpImpulse, ForceMode.Impulse);
        }
    }

    private bool IsGrounded()
    {
        // Bottom of the collider
        float bottom = col.bounds.min.y;

        // Start slightly above the bottom
        Vector3 origin = new(transform.position.x, bottom + 0.05f, transform.position.z);

        // Cast downward a small distance
        float checkDistance = 0.1f;

        return Physics.Raycast(origin, Vector3.down, checkDistance);
    }
}
