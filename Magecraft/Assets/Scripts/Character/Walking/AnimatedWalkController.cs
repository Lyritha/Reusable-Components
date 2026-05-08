using UnityEngine;

public class AnimatedWalkController : WalkController
{
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
            animator.SetFloat("Velocity/X", normalizedVelocity.x);
            animator.SetFloat("Velocity/Z", normalizedVelocity.y);
            animator.SetFloat("Velocity/Magnitude", normalizedVelocity.magnitude);
        }
    }
}
