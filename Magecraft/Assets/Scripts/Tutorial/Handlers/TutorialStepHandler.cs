using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.InputSystem;

public class TutorialStepHandler
{

    private TutorialStep currentStep;
    private Dictionary<InputAction, InputActionData> allActions = new();
    private Dictionary<InputAction, InputActionData> currentDeviceActions = new();

    public bool IsCompleted => currentDeviceActions.Values.All(c => c.HasBeenCompleted);

    public Action<TutorialStepData> OnDeviceChanged;
    public Action<InputAction> OnStepCompleted;

    public bool TrySetNewStep(TutorialStep step, out TutorialStepData data)
    {
        data = new();

        if (step == currentStep) return false;
        Cleanup();

        currentStep = step;

        BuildActions();

        RefreshActiveSet();

        data = GetSnapshot();
        return true;
    }

    public void RefreshForDevice()
    {
        if (currentStep == null) return;
        RefreshActiveSet();
        OnDeviceChanged?.Invoke(GetSnapshot());
    }

    private void BuildActions()
    {
        foreach ((string name, string path) in InputDeviceTracker.GetAllBindings(currentStep.input.action))
        {
            InputAction action = new(name: name, binding: path);
            action.performed += _ => OnActionPerformed(action);
            action.Enable();
            allActions[action] = new(false, InputNames.GetCleanBindingName(path, name));
        }
    }

    private void RefreshActiveSet()
    {
        currentDeviceActions.Clear();

        var activeBindings = InputDeviceTracker
            .GetBindingsForLastDevice(currentStep.input.action)
            .Select(b => b.path)
            .ToHashSet();

        foreach (var (action, completed) in allActions)
            if (activeBindings.Contains(action.bindings[0].effectivePath))
                currentDeviceActions[action] = completed;
    }

    private void OnActionPerformed(InputAction action)
    {
        if (!allActions.ContainsKey(action)) return;

        allActions[action].HasBeenCompleted = true;
        if (currentDeviceActions.ContainsKey(action)) currentDeviceActions[action].HasBeenCompleted = true;

        OnStepCompleted?.Invoke(action);
    }

    private TutorialStepData GetSnapshot() => new()
    {
        Prompt = GetPrompt(),
        AllActions = allActions,
        CurrentDeviceActions = currentDeviceActions
    };

    private string GetPrompt()
    {
        string bindingDisplay = string.Join(", ", InputDeviceTracker
            .GetBindingsForLastDevice(currentStep.input.action)
            .Select(b => InputNames.GetCleanBindingName(b.path, b.name)));

        return currentStep.prompt.Replace("<Input>", bindingDisplay);
    }

    public void Cleanup()
    {
        foreach (InputAction action in allActions.Keys)
        {
            action.Disable();
            action.Dispose();
        }

        allActions.Clear();
        currentDeviceActions.Clear();
        currentStep = null;
    }
}
