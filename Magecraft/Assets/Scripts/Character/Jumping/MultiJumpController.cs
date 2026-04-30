using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class MultiJumpController : MonoBehaviour, ICharacterJump
{
    [SerializeField]
    private float jumpImpulse = 180f;
    [SerializeField]
    private int maxJumps = 2;

    private Rigidbody rb;
    private Collider col;

    private int jumpsUsed = 0;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
    }

    public void OnJump()
    {
        if (CanJump())
        {
            Vector3 vel = rb.linearVelocity;
            if (vel.y < 0) vel.y = 0;
            rb.linearVelocity = vel;

            rb.AddForce(Vector3.up * jumpImpulse, ForceMode.Impulse);
            jumpsUsed++;
        }
    }

    private bool CanJump()
    {
        if (IsGrounded())
        {
            jumpsUsed = 0; // reset jumps when grounded
            return true;
        }

        // Allow one extra jump in the air
        return jumpsUsed < maxJumps;
    }

    private bool IsGrounded()
    {
        float bottom = col.bounds.min.y;
        Vector3 origin = new(transform.position.x, bottom + 0.05f, transform.position.z);

        // Small raycast to detect ground
        return Physics.Raycast(origin, Vector3.down, 0.1f);
    }
}
