using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class JumpController : InputListener
{
    [SerializeField]
    protected float jumpImpulse = 180f;

    protected Rigidbody rb;
    protected Collider col;

    protected void Awake()
    {
        // handles component exclucivity, copies base class info
        foreach (JumpController j in GetComponents<JumpController>())
        {
            if (j != this)
            {
                CopyBaseFieldsFrom(j);
                Destroy(j);
            }
        }


        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();

        AddSubscription(e => e.OnJump += OnJump, e => e.OnJump -= OnJump);
    }

    protected virtual void OnJump()
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

    protected bool IsGrounded()
    {
        float bottom = col.bounds.min.y;

        Vector3 origin = new(transform.position.x, bottom + 0.05f, transform.position.z);
        float checkDistance = 0.1f;

        return Physics.Raycast(origin, Vector3.down, checkDistance);
    }

    protected void CopyBaseFieldsFrom(JumpController other)
    {
        jumpImpulse = other.jumpImpulse;
    }

}
