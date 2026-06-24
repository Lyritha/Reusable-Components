using System;
using UnityEngine;
using UnityEngine.InputSystem;
using CallbackContext = UnityEngine.InputSystem.InputAction.CallbackContext;


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
        Bind(actions.Player.Scroll, OnScroll);

        actions.Player.Interact.performed += OnInteractInput;
        actions.Player.Tab.performed += OnTabInput;
        actions.Player.Jump.performed += OnJumpInput;
        actions.Player.NumberKey.performed += OnNumberSelectedInput;
        actions.Player.Inventory.performed += OnInventoryInput;
    }

    private void OnDisable()
    {
        Unbind(actions.Player.Move, OnMoveInput);
        Unbind(actions.Player.Look, OnLookInput);
        Unbind(actions.Player.Sprint, OnSprintInput);
        Unbind(actions.Player.Attack, OnAttackInput);
        Unbind(actions.Player.SecondaryAttack, OnSecondaryAttackInput);
        Unbind(actions.Player.Scroll, OnScroll);

        actions.Player.Interact.performed -= OnInteractInput;
        actions.Player.Tab.performed -= OnTabInput;
        actions.Player.Jump.performed -= OnJumpInput;
        actions.Player.NumberKey.performed -= OnNumberSelectedInput;
        actions.Player.Inventory.performed -= OnInventoryInput;

        actions.Disable();
    }


    private void Bind(InputAction action, Action<CallbackContext> callback)
    {
        action.performed += callback;
        action.canceled += callback;
    }
    private void Unbind(InputAction action, Action<CallbackContext> callback)
    {
        action.performed -= callback;
        action.canceled -= callback;
    }


    // input callbacks that raise the appropriate events with the correct value types
    private void OnMoveInput(CallbackContext ctx) => Move.Raise(GetVector2(ctx), ActiveLayer);
    private void OnLookInput(CallbackContext ctx) => LookDelta.Raise(GetVector2(ctx), ActiveLayer);
    private void OnSprintInput(CallbackContext ctx) => Sprint.Raise(GetBool(ctx), ActiveLayer);
    private void OnJumpInput(CallbackContext ctx) => Jump.Raise(ActiveLayer);
    private void OnAttackInput(CallbackContext ctx) => PrimaryMouse.Raise(GetBool(ctx), ActiveLayer);
    private void OnSecondaryAttackInput(CallbackContext ctx) => SecondaryMouse.Raise(GetBool(ctx), ActiveLayer);
    private void OnInteractInput(CallbackContext ctx) => Interact.Raise(ActiveLayer);
    private void OnInventoryInput(CallbackContext ctx) => InventoryButton.Raise(ActiveLayer);
    private void OnTabInput(CallbackContext ctx) => Tab.Raise(ActiveLayer);
    private void OnNumberSelectedInput(CallbackContext ctx) => NumberSelected.Raise(GetNumber(ctx), ActiveLayer);
    private void OnScroll(CallbackContext ctx) => Scroll.Raise((int)GetFloat(ctx), ActiveLayer);


    // utility functions to convert input values to the appropriate types for events
    private Vector2 GetVector2(CallbackContext ctx) => ctx.ReadValue<Vector2>();
    private float GetFloat(CallbackContext ctx) => ctx.ReadValue<float>();
    private bool GetBool(CallbackContext ctx) => ctx.ReadValue<float>() > 0.5f;
    private int GetNumber(CallbackContext ctx) => int.Parse(ctx.control.displayName);
}
