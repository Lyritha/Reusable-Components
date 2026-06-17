using System;
using UnityEngine;

public class RotateTowardsTarget : MonoBehaviour
{
    [SerializeField]
    private bool lockToYRotation = true;
    [SerializeField]
    private uint entityId = 0;

    private EntityController entityController;

    private void Start()
    {
        if (EntityController.TryGet(entityId, out EntityController result)) entityController = result;
    }

    private void Update()
    {
        if (entityController == null)
        {
            if (EntityController.TryGet(entityId, out EntityController result)) entityController = result;
            else return;
        }

        Vector3 currentPos = transform.position;
        Vector3 targetPos = entityController.transform.position;

        if (lockToYRotation) targetPos.y = currentPos.y;

        Vector3 dir = currentPos - targetPos;
        if (dir.sqrMagnitude > 0.0001f) transform.rotation = Quaternion.LookRotation(dir.normalized);
    }
}
