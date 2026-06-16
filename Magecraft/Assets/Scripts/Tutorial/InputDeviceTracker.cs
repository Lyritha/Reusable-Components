using System;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class InputDeviceTracker : Singleton<InputDeviceTracker>
{
    public static InputDevice LastUsedDevice { get; private set; }
    public static Action OnInputSourceChanged;


    private void OnEnable() => InputSystem.onActionChange += OnActionChange;
    private void OnDisable() => InputSystem.onActionChange -= OnActionChange;


    private void OnActionChange(object obj, InputActionChange change)
    {
        if (change != InputActionChange.ActionPerformed) return;
        if (obj is InputAction action)
        {
            LastUsedDevice = action.activeControl?.device;
            OnInputSourceChanged?.Invoke();
        }
    }

    public static List<(string name, string path)> GetBindingsForLastDevice(InputAction action)
    {
        bool isGamepad = LastUsedDevice is Gamepad;
        var bindings = action.bindings;

        for (int i = 0; i < bindings.Count; i++)
        {
            var binding = bindings[i];

            if (binding.isComposite)
            {
                if (i + 1 >= bindings.Count) continue;

                bool isGamepadComposite = IsGamepadPath(bindings[i + 1].effectivePath);
                if (isGamepad != isGamepadComposite) continue;

                // Return each child as a separate entry
                var parts = new List<(string name, string path)>();
                int j = i + 1;
                while (j < bindings.Count && bindings[j].isPartOfComposite)
                {
                    parts.Add((
                        name: InputControlPath.ToHumanReadableString(
                            bindings[j].effectivePath,
                            InputControlPath.HumanReadableStringOptions.OmitDevice
                        ),
                        path: bindings[j].effectivePath
                    ));
                    j++;
                }
                return parts;
            }

            if (binding.isPartOfComposite) continue;

            if (isGamepad == IsGamepadPath(binding.effectivePath))
            {
                return new List<(string name, string path)>
            {
                (
                    name: InputControlPath.ToHumanReadableString(
                        binding.effectivePath,
                        InputControlPath.HumanReadableStringOptions.OmitDevice
                    ),
                    path: binding.effectivePath
                )
            };
            }
        }

        // Fallback
        return new List<(string name, string path)>
    {
        (
            name: InputControlPath.ToHumanReadableString(
                action.bindings[0].effectivePath,
                InputControlPath.HumanReadableStringOptions.OmitDevice
            ),
            path: action.bindings[0].effectivePath
        )
    };
    }
    public static List<(string name, string path)> GetAllBindings(InputAction action)
    {
        var result = new List<(string name, string path)>();
        var bindings = action.bindings;

        for (int i = 0; i < bindings.Count; i++)
        {
            var binding = bindings[i];

            if (binding.isComposite)
            {
                int j = i + 1;
                while (j < bindings.Count && bindings[j].isPartOfComposite)
                {
                    result.Add((
                        name: InputControlPath.ToHumanReadableString(
                            bindings[j].effectivePath,
                            InputControlPath.HumanReadableStringOptions.OmitDevice
                        ),
                        path: bindings[j].effectivePath
                    ));
                    j++;
                }
                continue;
            }

            if (binding.isPartOfComposite) continue;

            if (!string.IsNullOrEmpty(binding.effectivePath))
            {
                result.Add((
                    name: InputControlPath.ToHumanReadableString(
                        binding.effectivePath,
                        InputControlPath.HumanReadableStringOptions.OmitDevice
                    ),
                    path: binding.effectivePath
                ));
            }
        }

        return result;
    }

    public static bool IsGamepadPath(string path)
    {
        return path.StartsWith("<Gamepad>")
               || path.StartsWith("<DualShock>")
               || path.StartsWith("<XInputController>");
    }
}