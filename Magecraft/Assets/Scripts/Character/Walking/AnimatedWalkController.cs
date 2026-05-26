using UnityEngine;

public class AnimatedWalkController : WalkController
{
    private static readonly int VelocityMagnitudeHash = Animator.StringToHash("Velocity/Magnitude");
    private static readonly int VelocityZHash = Animator.StringToHash("Velocity/Z");
    private static readonly int VelocityXHash = Animator.StringToHash("Velocity/X");

    [SerializeField]
    private Animator animator;

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        UpdateAnimator(localVelocity);
    }

    private void UpdateAnimator(Vector2 velocity)
    {
        Vector2 normalizedVelocity = velocity;

        normalizedVelocity.x = normalizedVelocity.x > 0 ? normalizedVelocity.x / maxSpeed.right : normalizedVelocity.x / maxSpeed.left;
        normalizedVelocity.y = normalizedVelocity.y > 0 ? normalizedVelocity.y / maxSpeed.forward : normalizedVelocity.y / maxSpeed.backward;

        if (animator != null)
        {
            animator.SetFloat(VelocityXHash, normalizedVelocity.x);
            animator.SetFloat(VelocityZHash, normalizedVelocity.y);
            animator.SetFloat(VelocityMagnitudeHash, normalizedVelocity.magnitude);
        }
    }
}
