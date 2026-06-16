using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "TutorialStep", menuName = "Scriptable Objects/TutorialStep")]
public class TutorialStep : ScriptableObject
{
    public InputActionReference requiredPress;
    [Tooltip("Use <Input> where it should show the inputs")]
    public string prompt;

    private Dictionary<InputAction, bool> requiredActions = new();
    private Dictionary<InputAction, bool> activeRequiredActions = new();

    public bool IsCompleted => activeRequiredActions.Values.All(c => c);
    public Action InputCompleted;

    public Dictionary<InputAction, bool> ActiveRequiredActions => activeRequiredActions;
    public Dictionary<InputAction, bool> RequiredActions => requiredActions;

    public void Initialize()
    {
        requiredActions.Clear();

        if (requiredPress == null) return;

        foreach (var (name, path) in InputDeviceTracker.GetAllBindings(requiredPress.action))
        {
            InputAction childAction = new(name: name, binding: path);
            childAction.performed += ctx => OnActionPerformed(childAction);
            childAction.Enable();

            requiredActions[childAction] = false;
        }

        RefreshActiveSet();
    }

    public void RefreshActiveSet()
    {
        activeRequiredActions.Clear();

        var activeBindings = InputDeviceTracker
            .GetBindingsForLastDevice(requiredPress.action)
            .Select(b => b.path)
            .ToHashSet();

        foreach (var action in requiredActions.Keys)
        {
            if (activeBindings.Contains(action.bindings[0].effectivePath))
                activeRequiredActions.Add(action, false);
        }
    }

    public void Cleanup()
    {
        foreach (var action in requiredActions.Keys)
        {
            action.Disable();
            action.Dispose();
        }
        requiredActions.Clear();
        activeRequiredActions.Clear();
    }

    private void OnActionPerformed(InputAction action)
    {
        if (requiredActions.ContainsKey(action))
        {
            InputCompleted?.Invoke();
            requiredActions[action] = true;
        }
    }

    public string GetPrompt()
    {
        if (requiredPress == null) return prompt;

        string bindingDisplay = string.Join(", ", InputDeviceTracker
            .GetBindingsForLastDevice(requiredPress.action)
            .Select(b => b.name));

        return prompt.Replace("<Input>", bindingDisplay);
    }
}