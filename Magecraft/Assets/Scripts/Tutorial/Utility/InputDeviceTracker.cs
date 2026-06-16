using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

#if UNITY_EDITOR
using UnityEditor;
#endif

public static class InputDeviceTracker
{
    public static InputDevice LastUsedDevice { get; private set; }
    public static Action OnInputSourceChanged;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Initialize()
    {
        InputSystem.onActionChange -= OnActionChange; // prevent double-register
        InputSystem.onActionChange += OnActionChange;

#if UNITY_EDITOR
        EditorApplication.playModeStateChanged -= OnPlayModeChanged;
        EditorApplication.playModeStateChanged += OnPlayModeChanged;
#endif
    }

#if UNITY_EDITOR
    private static void OnPlayModeChanged(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.ExitingPlayMode)
        {
            InputSystem.onActionChange -= OnActionChange;
            LastUsedDevice = null;
            OnInputSourceChanged = null;
        }
    }
#endif

    private static void OnActionChange(object obj, InputActionChange change)
    {
        if (change != InputActionChange.ActionPerformed) return;
        if (obj is InputAction action)
        {
            var device = action.activeControl?.device;
            if (device == null || device == LastUsedDevice) return;

            LastUsedDevice = device;
            OnInputSourceChanged?.Invoke();
        }
    }

    public static List<(string name, string path)> GetBindingsForLastDevice(InputAction action)
    {
        bool isGamepad = LastUsedDevice is Gamepad;
        var bindings = action.bindings;

        for (int i = 0; i < bindings.Count; i++)
        {
            InputBinding binding = bindings[i];

            if (binding.isComposite)
            {
                if (i + 1 >= bindings.Count) continue;

                bool isGamepadComposite = IsGamepadPath(bindings[i + 1].effectivePath);
                if (isGamepad != isGamepadComposite) continue;

                var parts = new List<(string name, string path)>();
                int j = i + 1;
                while (j < bindings.Count && bindings[j].isPartOfComposite)
                {
                    parts.Add((
                        name: InputControlPath.ToHumanReadableString( bindings[j].effectivePath, InputControlPath.HumanReadableStringOptions.OmitDevice),
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
                        name: InputControlPath.ToHumanReadableString(binding.effectivePath, InputControlPath.HumanReadableStringOptions.OmitDevice),
                        path: binding.effectivePath
                    )
                };
            }
        }
        return new List<(string name, string path)>
    {
        (
            name: InputControlPath.ToHumanReadableString( action.bindings[0].effectivePath,
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

    public static bool IsGamepadPath(string path) =>
        path.StartsWith("<Gamepad>") ||
        path.StartsWith("<DualShock>") ||
        path.StartsWith("<XInputController>");
}