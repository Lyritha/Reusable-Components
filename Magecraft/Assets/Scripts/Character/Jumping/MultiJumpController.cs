using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class MultiJumpController : JumpController
{
    [SerializeField]
    private int maxJumps = 2;

    private int jumpsUsed = 0;

    protected override void OnJump()
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
            jumpsUsed = 0;
            return true;
        }

        return jumpsUsed < maxJumps;
    }
}
