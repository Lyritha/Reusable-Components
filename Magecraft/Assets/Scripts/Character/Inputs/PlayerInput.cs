using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class PlayerInput : MonoBehaviour, ICharacterInput
{
    private InputSystem_Actions actions;

    public event Action<Vector2> MoveEvent;

    public event Action<Vector2> LookEvent;
    public event Action SwitchLookEvent;

    public event Action<bool> SprintEvent;

    public event Action JumpEvent;
    public event Action<bool> AttackEvent;
    public event Action<int> NumberSelectEvent;

    private void Awake()
    {
        actions = new InputSystem_Actions();
    }

    private void OnEnable()
    {
        actions.Enable();

        actions.Player.Move.performed += OnMove;
        actions.Player.Move.canceled += OnMove;

        actions.Player.Look.performed += OnLook;
        actions.Player.Look.canceled += OnLook;

        actions.Player.SwitchLook.performed += OnSwitchLook;
        actions.Player.SwitchLook.canceled += OnSwitchLook;

        actions.Player.Sprint.performed += OnSprint;
        actions.Player.Sprint.canceled += OnSprint;

        actions.Player.Jump.performed += OnJump;

        actions.Player.Attack.performed += OnAttack;
        actions.Player.Attack.canceled += OnAttack;

        actions.Player.NumberKey.performed += OnNumberSelected;
    }

    private void OnDisable()
    {
        actions.Player.Move.performed -= OnMove;
        actions.Player.Move.canceled -= OnMove;

        actions.Player.Look.performed -= OnLook;
        actions.Player.Look.canceled -= OnLook;

        actions.Player.SwitchLook.performed -= OnSwitchLook;
        actions.Player.SwitchLook.canceled -= OnSwitchLook;

        actions.Player.Sprint.performed -= OnSprint;
        actions.Player.Sprint.canceled -= OnSprint;

        actions.Player.Jump.performed -= OnJump;

        actions.Player.Attack.performed -= OnAttack;
        actions.Player.Attack.canceled -= OnAttack;

        actions.Player.NumberKey.performed -= OnNumberSelected;

        actions.Disable();
    }

    private void OnSwitchLook(InputAction.CallbackContext ctx)
    {
        SwitchLookEvent?.Invoke();
    }

    private void OnLook(InputAction.CallbackContext ctx)
    {
        Vector2 Look = ctx.ReadValue<Vector2>();
        LookEvent?.Invoke(Look);
    }

    private void OnMove(InputAction.CallbackContext ctx)
    {
        Vector2 Move = ctx.ReadValue<Vector2>();
        MoveEvent?.Invoke(Move);
    }

    private void OnJump(InputAction.CallbackContext ctx) => JumpEvent?.Invoke();

    private void OnAttack(InputAction.CallbackContext ctx)
    {
        bool isAttacking = ctx.ReadValue<float>() > 0.5f;
        AttackEvent?.Invoke(isAttacking);
    }

    private void OnSprint(InputAction.CallbackContext ctx)
    {
        bool isSprinting = ctx.ReadValue<float>() > 0.5f;
        SprintEvent?.Invoke(isSprinting);
    }

    private void OnNumberSelected(InputAction.CallbackContext ctx)
    {
        string numberName = ctx.control.displayName;
        int number = int.Parse(numberName);
        NumberSelectEvent?.Invoke(number);
    }
}
