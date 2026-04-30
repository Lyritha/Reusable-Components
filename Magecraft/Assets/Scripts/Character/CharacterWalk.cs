using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CharacterWalk : MonoBehaviour, ICharacterMove
{
    [SerializeField]
    private Vector2 moveSpeed = new(2, 3f); // m/s
    [SerializeField]
    private float acceleration = 6f;

    private Vector2 dir = Vector2.zero;
    private Rigidbody rb;

    public Vector2 MoveSpeed { get => moveSpeed; set => moveSpeed = value; }

    private void Awake() => rb = GetComponent<Rigidbody>();

    public void Move(Vector2 dir) => this.dir = dir;

    private void FixedUpdate()
    {
        // Determine target velocity
        Vector3 targetVel;

        if (dir.sqrMagnitude < 0.1f)
        {
            // No input → stop horizontal movement but keep gravity
            targetVel = new Vector3(0, rb.linearVelocity.y, 0);
        }
        else
        {
            // Convert input to local movement scaled by m/s
            Vector2 localDir2D = dir.normalized * moveSpeed;
            Vector3 worldDir = transform.forward * localDir2D.y + transform.right * localDir2D.x;

            targetVel = new Vector3(worldDir.x, rb.linearVelocity.y, worldDir.z);
        }

        // Smooth acceleration toward target velocity
        rb.linearVelocity = Vector3.MoveTowards(
            rb.linearVelocity,
            targetVel,
            acceleration
        );
    }
}
