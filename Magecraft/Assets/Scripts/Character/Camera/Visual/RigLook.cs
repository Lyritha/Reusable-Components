using UnityEngine;
using UnityEngine.Animations.Rigging;

public class RigLook : MonoBehaviour
{
    private CinemachineLook panTiltSource;

    private MultiAimConstraint constraint;
    private Transform head;
    private Transform target;

    private void Awake()
    {
        constraint = GetComponentInChildren<MultiAimConstraint>();

        head = constraint.data.constrainedObject;
        target = constraint.data.sourceObjects[0].transform;

        constraint.data.constrainedXAxis = true;
        constraint.data.constrainedYAxis = true;
        constraint.data.constrainedZAxis = true;

        target.SetParent(null, true);
    }

    private void Update()
    {
        if (panTiltSource == null) panTiltSource = GetComponent<CinemachineLook>();

        if (panTiltSource != null)
        {
            Vector3 camForward = panTiltSource.PanTilt.transform.forward;
            target.position = head.position + camForward * 1f;
        }
    }

}
