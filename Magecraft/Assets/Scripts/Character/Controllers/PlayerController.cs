using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : EntityController
{
    private InputSystem_Actions actions;

    protected override void Awake()
    {
        base.Awake();
        actions = new InputSystem_Actions();
    }

    private void OnEnable()
    {
        actions.Enable();

        Bind(actions.Player.Move, OnMoveInput);
        Bind(actions.Player.Look, OnLookInput);
        Bind(actions.Player.Sprint, OnSprintInput);
        Bind(actions.Player.Attack, OnAttackInput);
        Bind(actions.Player.SecondaryAttack, OnSecondaryAttackInput);

        actions.Player.Interact.performed += OnInteractInput;
        actions.Player.Tab.performed += OnTabInput;
        actions.Player.Jump.performed += OnJumpInput;
        actions.Player.NumberKey.performed += OnNumberSelectedInput;
    }

    private void OnDisable()
    {
        Unbind(actions.Player.Move, OnMoveInput);
        Unbind(actions.Player.Look, OnLookInput);
        Unbind(actions.Player.Sprint, OnSprintInput);
        Unbind(actions.Player.Attack, OnAttackInput);
        Unbind(actions.Player.SecondaryAttack, OnSecondaryAttackInput);

        actions.Player.Interact.performed -= OnInteractInput;
        actions.Player.Tab.performed -= OnTabInput;
        actions.Player.Jump.performed -= OnJumpInput;
        actions.Player.NumberKey.performed -= OnNumberSelectedInput;

        actions.Disable();
    }


    private void Bind(InputAction action, Action<InputAction.CallbackContext> callback)
    {
        action.performed += callback;
        action.canceled += callback;
    }
    private void Unbind(InputAction action, Action<InputAction.CallbackContext> callback)
    {
        action.performed -= callback;
        action.canceled -= callback;
    }


    private void OnMoveInput(InputAction.CallbackContext ctx) => RaiseMove(ctx.ReadValue<Vector2>());
    private void OnLookInput(InputAction.CallbackContext ctx) => RaiseLookDelta(ctx.ReadValue<Vector2>());
    private void OnSprintInput(InputAction.CallbackContext ctx) => RaiseSprint(ctx.ReadValue<float>() > 0.5f);
    private void OnJumpInput(InputAction.CallbackContext ctx) => RaiseJump();
    private void OnAttackInput(InputAction.CallbackContext ctx) => RaisePrimaryMouse(ctx.ReadValue<float>() > 0.5f);
    private void OnSecondaryAttackInput(InputAction.CallbackContext ctx) => RaiseSecondaryMouse(ctx.ReadValue<float>() > 0.5f);
    private void OnInteractInput(InputAction.CallbackContext ctx) => RaiseInteract();

    private void OnTabInput(InputAction.CallbackContext ctx) => RaiseTab();
    private void OnNumberSelectedInput(InputAction.CallbackContext ctx) => RaiseNumberSelected(int.Parse(ctx.control.displayName));
}
