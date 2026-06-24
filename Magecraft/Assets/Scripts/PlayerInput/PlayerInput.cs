using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
    [SerializeField]
    private InputActionReference input;

    public Action OnPerformed;

    public InputAction Action { get; private set; }

    private void Awake()
    {
        if (input == null || input.action == null)
        {
            Debug.LogError("PlayerInput: No InputActionReference assigned.");
            return;
        }

        Action = input.action;
    }

    private void OnEnable()
    {
        if (Action != null) Action.performed += OnActionPerformed;
    }

    private void OnDisable()
    {
        if (Action != null) Action.performed -= OnActionPerformed;
    }

    private void OnActionPerformed(InputAction.CallbackContext ctx) => OnPerformed?.Invoke();
    public T ReadValue<T>() where T : struct => Action == null ? default : Action.ReadValue<T>();
}
