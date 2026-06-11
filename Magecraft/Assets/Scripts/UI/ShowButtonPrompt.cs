using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class ShowButtonPrompt : MonoBehaviour
{
    [Header("Input Action")]
    public InputActionReference action;

    [Header("UI")]
    public TMP_Text text;

    private void OnEnable()
    {
        UpdatePrompt();
        InputSystem.onActionChange += OnActionChange;
    }

    private void OnDisable()
    {
        InputSystem.onActionChange -= OnActionChange;
    }

    private void OnActionChange(object obj, InputActionChange change)
    {
        // Only update when this specific action changes
        if (obj == action.action)
            UpdatePrompt();
    }

    private void UpdatePrompt()
    {
        if (action == null || action.action == null)
            return;

        // This automatically respects runtime overrides, rebinding, device changes, etc.
        string display = action.action.GetBindingDisplayString();

        text.text = display;
    }
}
