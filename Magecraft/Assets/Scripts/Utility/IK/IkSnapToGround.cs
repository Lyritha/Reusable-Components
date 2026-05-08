using UnityEngine;

public class FootIKTarget : MonoBehaviour
{
    [Header("Ground check")]
    [SerializeField]
    private float groundedCheckRadius = 0.05f;
    [SerializeField]
    private float groundedCheckHeight = 0.1f;

    [Header("Placement")]
    [SerializeField]
    private float raycastHeight = 0.5f;
    [SerializeField]
    private float footOffset = 0.02f;
    [SerializeField]
    private float smooth = 10f;
    [SerializeField]
    private LayerMask groundMask;

    private Vector3 targetPos;
    private Quaternion targetRot;
    private Quaternion initialRot;

    [SerializeField]
    private Transform animFoot; // animated foot bone (parent)

    private void Awake()
    {
        // Store initial local rotation offset
        initialRot = transform.rotation;

        // Initialize target pose
        targetPos = transform.position;
        targetRot = transform.rotation;
    }

    private void Update()
    {
        // STEP 1 — Small grounded check (avoid overriding jumps)
        if (!IsFootGrounded())
        {
            targetPos = animFoot.position;
            targetRot = animFoot.rotation;
        }
        else
        {
            // STEP 2 — Raycast from above to find ground
            Vector3 origin = animFoot.position + Vector3.up * raycastHeight;

            if (Physics.Raycast(origin, Vector3.down, out RaycastHit hit, raycastHeight * 2f, groundMask))
            {
                // STEP 3 — Position foot on ground + offset
                targetPos = hit.point + hit.normal * footOffset;

                // STEP 4 — Align foot to surface normal while preserving forward direction
                Vector3 forward = animFoot.forward;
                Vector3 projectedForward = Vector3.ProjectOnPlane(forward, hit.normal).normalized;

                if (projectedForward.sqrMagnitude < 0.0001f)
                    projectedForward = animFoot.forward;

                Quaternion groundRot = Quaternion.LookRotation(projectedForward, hit.normal);

                // Apply initial local rotation offset
                targetRot = groundRot * initialRot;
            }
            else
            {
                // No ground hit → fallback to animation
                targetPos = animFoot.position;
                targetRot = animFoot.rotation * initialRot;
            }
        }

        // STEP 5 — Smooth additive blending
        transform.SetPositionAndRotation(
            Vector3.Lerp(transform.position, targetPos, Time.deltaTime * smooth),
            Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * smooth)
        );
    }

    private bool IsFootGrounded()
    {
        Vector3 center = animFoot.position + Vector3.down * groundedCheckHeight;
        return Physics.CheckSphere(center, groundedCheckRadius, groundMask, QueryTriggerInteraction.Ignore);
    }

    private void OnDrawGizmosSelected()
    {
        if (animFoot == null) animFoot = transform.parent;

        // Grounded check sphere
        Gizmos.color = Color.yellow;
        Vector3 center = animFoot.position + Vector3.down * groundedCheckHeight;
        Gizmos.DrawWireSphere(center, groundedCheckRadius);

        // Raycast visualization
        Gizmos.color = Color.cyan;
        Vector3 origin = animFoot.position + Vector3.up * raycastHeight;
        Gizmos.DrawLine(origin, origin + Vector3.down * (raycastHeight * 2f));
    }
}
