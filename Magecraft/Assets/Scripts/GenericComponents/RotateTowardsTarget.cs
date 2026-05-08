using System;
using UnityEngine;

public class RotateTowardsTarget : MonoBehaviour
{
    [SerializeField]
    private bool lockToYRotation = true;
    [SerializeField]
    private uint characterControllerId = 0;

    private CharacterController characterController;

    private void Start()
    {
        if (CharacterController.TryGet(characterControllerId, out CharacterController result)) characterController = result;
    }

    private void Update()
    {
        if (characterController == null)
        {
            if (CharacterController.TryGet(characterControllerId, out CharacterController result)) characterController = result;
            else return;
        }

        Vector3 currentPos = transform.position;
        Vector3 targetPos = characterController.transform.position;

        if (lockToYRotation) targetPos.y = currentPos.y;

        Vector3 dir = targetPos - currentPos;
        if (dir.sqrMagnitude > 0.0001f) transform.rotation = Quaternion.LookRotation(dir.normalized);
    }
}
