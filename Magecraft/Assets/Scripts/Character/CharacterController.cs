using System;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
    private ICharacterInput input = null;
    private ICharacterInput lastInput = null;

    private ICharacterMove characterMove = null;

    private ICharacterLook characterLook = null;
    private CameraMode characterCameraMode = CameraMode.FirstPerson;

    private ICharacterJump characterJump = null;
    private ICharacterSprint characterSprint = null;

    private IWeapon characterWeapon = null;

    private void OnMove(Vector2 dir)
    {
        characterMove = GetComponent<ICharacterMove>();

        // disable movement when in free look third person mode
        if (characterCameraMode == CameraMode.FreeLookThirdPerson)
        {
            characterMove?.Move(Vector2.zero);
            return;
        }

        characterMove?.Move(dir);
    }


    private void OnLook(Vector2 dir)
    {
        characterLook = GetComponent<ICharacterLook>();
        characterLook?.OnLook(dir);
    }

    private void OnSwitchLook()
    {
        if (characterLook == null) return;

        bool switchingToFreeLook = characterLook is FirstPersonLook;
        Type nextType = switchingToFreeLook ? typeof(ThirdPersonOrbitalLook) : typeof(FirstPersonLook);

        Destroy((Component)characterLook);
        characterLook = (ICharacterLook)gameObject.AddComponent(nextType);

        // Update camera mode enum
        characterCameraMode = switchingToFreeLook
            ? CameraMode.FreeLookThirdPerson
            : CameraMode.FirstPerson;
    }


    private void OnSprint(bool isSprinting)
    {
        characterSprint = GetComponent<ICharacterSprint>();
        characterSprint?.OnSprint(isSprinting);
    }

    private void OnJump()
    {
        characterJump = GetComponent<ICharacterJump>();
        characterJump?.OnJump();
    }

    private void OnAttack(bool isAttacking)
    {
        characterWeapon = GetComponentInChildren<IWeapon>();
        characterWeapon?.Use(isAttacking);
    }

    private void Update()
    {
        input = GetComponent<ICharacterInput>();

        if (input != lastInput)
        {
            SwapInput(lastInput, input);
            lastInput = input;
        }
    }

    private void SwapInput(ICharacterInput oldInput, ICharacterInput newInput)
    {
        if (oldInput != null)
        {
            oldInput.MoveEvent -= OnMove;
            oldInput.JumpEvent -= OnJump;
            oldInput.SprintEvent -= OnSprint;
            oldInput.LookEvent -= OnLook;
            oldInput.SwitchLookEvent -= OnSwitchLook;
            oldInput.AttackEvent -= OnAttack;
        }

        if (newInput != null)
        {
            newInput.MoveEvent += OnMove;
            newInput.JumpEvent += OnJump;
            newInput.SprintEvent += OnSprint;
            newInput.LookEvent += OnLook;
            newInput.SwitchLookEvent += OnSwitchLook;
            newInput.AttackEvent += OnAttack;
        }
    }

    private void OnDisable()
    {
        // Clean up if disabled
        if (lastInput != null)
        {
            lastInput.MoveEvent -= OnMove;
            lastInput.JumpEvent -= OnJump;
            lastInput.SprintEvent -= OnSprint;
            lastInput.LookEvent -= OnLook;
            lastInput.SwitchLookEvent -= OnSwitchLook;
        }
    }

    private enum CameraMode
    {
        FirstPerson,
        FreeLookThirdPerson
    }
}
