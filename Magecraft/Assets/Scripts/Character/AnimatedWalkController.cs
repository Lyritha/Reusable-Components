using UnityEngine;

public class AnimatedWalkController : MonoBehaviour, ICharacterMove
{
    [SerializeField]
    private Vector2 moveSpeed = new(1, 1f); // m/s
    [SerializeField]
    private float acceleration = 0.5f;
    [SerializeField]
    private Animator animator;

    private Vector2 dir = Vector2.zero;
    private Rigidbody rb;
    private Vector2 localVelocity = Vector2.zero;


    // allows other scripts to modify move speed without affecting inspector (base) value
    private Vector2 actualMoveSpeed = Vector2.zero;
    public Vector2 MoveSpeed { get => actualMoveSpeed; set => actualMoveSpeed = value; }

    private void Awake()
    {
        actualMoveSpeed = moveSpeed;
        rb = GetComponent<Rigidbody>();
    }

    public void Move(Vector2 dir) => this.dir = dir;

    private void FixedUpdate()
    {
        Vector2 localTargetVel;

        if (dir.sqrMagnitude < 0.1f) localTargetVel = Vector2.zero;
        else localTargetVel = dir.normalized * actualMoveSpeed;

        // calculate local velocity to make easier to use for animator
        localVelocity = Vector2.MoveTowards(
            localVelocity,
            localTargetVel,
            acceleration
        );

        // calculate velocity needed for rigidbody
        Vector3 worldVelocity = transform.forward * localVelocity.y + transform.right * localVelocity.x;
        worldVelocity.y = rb.linearVelocity.y;
        rb.linearVelocity = worldVelocity;

        // pass velocity to animator
        UpdateAnimator(localVelocity);
    }

    private void UpdateAnimator(Vector2 velocity)
    {
        Vector2 normalizedVelocity = velocity;
        normalizedVelocity.x /= moveSpeed.x;
        normalizedVelocity.y /= moveSpeed.y;


        if (animator != null)
        {
            animator.SetFloat("Velocity/X", normalizedVelocity.x);
            animator.SetFloat("Velocity/Z", normalizedVelocity.y);
            animator.SetFloat("Velocity/Magnitude", normalizedVelocity.magnitude);
        }
    }
}
