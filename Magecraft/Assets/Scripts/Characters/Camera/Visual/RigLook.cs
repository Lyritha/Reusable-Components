using UnityEngine;
using UnityEngine.Animations.Rigging;

[RequireComponent(typeof(MultiAimConstraint))]
public class RigLook : MonoBehaviour
{
    //private CinemachineLook panTiltSource;

    private MultiAimConstraint constraint;
    private Transform head;
    private Transform target;

    private Look look;

    private void Awake()
    {
        constraint = GetComponent<MultiAimConstraint>();
        look = GetComponentInParent<Look>();

        head = constraint.data.constrainedObject;
        target = constraint.data.sourceObjects[0].transform;

        constraint.data.constrainedXAxis = true;
        constraint.data.constrainedYAxis = true;
        constraint.data.constrainedZAxis = true;

        target.SetParent(null, true);
    }

    private void LateUpdate()
    {
        float pitch = Mathf.Clamp(look.Pitch, constraint.data.limits.x, constraint.data.limits.y);

        Vector3 camForward = Camera.main.transform.forward;
        target.position = head.position + camForward * 1f;
    }

}
