using System;
using UnityEngine;

public class RotateTowardsTarget : MonoBehaviour
{
    [SerializeField]
    private bool lockToYRotation = true;
    [SerializeField]
    private uint characterControllerId = 0;

    private EntityController characterController;

    private void Start()
    {
        if (EntityController.TryGet(characterControllerId, out EntityController result)) characterController = result;
    }

    private void Update()
    {
        if (characterController == null)
        {
            if (EntityController.TryGet(characterControllerId, out EntityController result)) characterController = result;
            else return;
        }

        Vector3 currentPos = transform.position;
        Vector3 targetPos = characterController.transform.position;

        if (lockToYRotation) targetPos.y = currentPos.y;

        Vector3 dir = targetPos - currentPos;
        if (dir.sqrMagnitude > 0.0001f) transform.rotation = Quaternion.LookRotation(dir.normalized);
    }
}
