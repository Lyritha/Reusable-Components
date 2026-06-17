using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
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
            allActions[action] = new(false, InputDeviceTracker.Sanitize(path, name));
        }
    }

    private void RefreshActiveSet()
    {
        currentDeviceActions.Clear();

        HashSet<string> activeBindings = InputDeviceTracker.GetBindingsLastDevice(currentStep.input.action).Select(b => b.path).ToHashSet();

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
        List<string> inputs = InputDeviceTracker.GetNamesLastDevice(currentStep.input.action, true);

        var (commonPrefix, strippedInputs) = ExtractCommonPrefix(inputs);
        bool hasCommonPrefix = !string.IsNullOrEmpty(commonPrefix) && strippedInputs.All(s => s.Length > 0);

        if (hasCommonPrefix) inputs = strippedInputs;

        string prompt;
        if (inputs.Count == 1) prompt = inputs[0];
        else if (hasCommonPrefix) prompt = $"{commonPrefix} {string.Join("/", inputs)}";
        else prompt = string.Join(inputs.Count == 2 ? " and " : ",", inputs);

        return currentStep.prompt.Replace("<Input>", $"\"{prompt}\"");
    }

    private (string prefix, List<string> names) ExtractCommonPrefix(List<string> names)
    {
        if (names.Count < 2) return ("", names);

        string prefix = names[0];
        foreach (string name in names)
        {
            int matchLength = 0;
            int maxLength = Mathf.Min(prefix.Length, name.Length);
            while (matchLength < maxLength && prefix[matchLength] == name[matchLength]) matchLength++;
            prefix = prefix[..matchLength];
            if (prefix.Length == 0) break;
        }

        int lastSpace = prefix.LastIndexOf(' ');
        prefix = lastSpace >= 0 ? prefix[..(lastSpace + 1)] : "";

        if (string.IsNullOrEmpty(prefix)) return ("", names);

        var stripped = names.Select(n => n[prefix.Length..]).ToList();
        return (prefix.TrimEnd(), stripped);
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
