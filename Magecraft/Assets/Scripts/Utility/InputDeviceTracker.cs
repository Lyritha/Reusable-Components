using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;


#if UNITY_EDITOR
using UnityEditor;
#endif

public static class InputDeviceTracker
{
    // keep track of device state changes
    public static InputDevice LastUsedDevice { get; private set; }
    public static Action OnInputSourceChanged;

    // useful for device tracking and cleaning up naming
    private enum Devices
    {
        Gamepad,
        Dualshock,
        XInputController,
        Mouse,
        Keyboard
    }
    private static readonly Dictionary<string, string> MouseBindings = new() {
        { "delta", "mouse" },
        { "left button", "left mouse" },
        { "right button", "right mouse" },
        { "scroll/up", "scroll up" },
        { "scroll/down", "scroll down" },
    };
    private static readonly Dictionary<string, string> ControllerBindings = new() {
        { "d-pad/down", "d-pad down" },
        { "d-pad/up", "d-pad up" },
        { "d-pad/left", "d-pad left" },
        { "d-pad/right", "d-pad right" },
    };


    // keep track of device changes
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
            InputDevice device = action.activeControl?.device;
            if (device == null || device == LastUsedDevice) return;

            LastUsedDevice = device;
            OnInputSourceChanged?.Invoke();
        }
    }



    // handle getting bindings and such
    public static List<(string name, string path)> GetBindingsForLastDevice(InputAction action, bool cleanName = false)
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
                    parts.Add(MakeBindingEntry(bindings[j].effectivePath, cleanName));
                    j++;
                }
                return parts;
            }

            if (binding.isPartOfComposite) continue;

            if (isGamepad == IsGamepadPath(binding.effectivePath))
            {
                return new List<(string name, string path)> { MakeBindingEntry(binding.effectivePath, cleanName) };
            }
        }

        return new List<(string name, string path)> { MakeBindingEntry(action.bindings[0].effectivePath, cleanName) };
    }
    public static List<(string name, string path)> GetAllBindings(InputAction action, bool cleanName = false)
    {
        List<(string name, string path)> result = new List<(string name, string path)>();
        ReadOnlyArray<InputBinding> bindings = action.bindings;

        for (int i = 0; i < bindings.Count; i++)
        {
            InputBinding binding = bindings[i];

            if (binding.isComposite)
            {
                int j = i + 1;
                while (j < bindings.Count && bindings[j].isPartOfComposite)
                {
                    result.Add(MakeBindingEntry(bindings[j].effectivePath, cleanName));
                    j++;
                }
                continue;
            }

            if (binding.isPartOfComposite) continue;

            if (!string.IsNullOrEmpty(binding.effectivePath))
                result.Add(MakeBindingEntry(binding.effectivePath, cleanName));
        }

        return result;
    }
    private static (string name, string path) MakeBindingEntry(string path, bool cleanName)
    {
        string rawName = InputControlPath.ToHumanReadableString(path, InputControlPath.HumanReadableStringOptions.OmitDevice);
        return (name: cleanName ? GetSanitizedBindingName(path, rawName) : rawName, path: path);
    }



    // get a binding name that is more human readable
    public static string GetSanitizedBindingName(string path, string name)
    {
        name = name.ToLower();

        if (IsDevice(path, Devices.Mouse)) return MouseBindings.TryGetValue(name, out string friendly) ? friendly : name;
        if (IsDevice(path, Devices.Keyboard)) return name;
        if (IsGamepadPath(path)) return ControllerBindings.TryGetValue(name, out string friendly) ? friendly : name;

        return name;
    }



    // get device and see if the path points to a specific device
    public static bool IsGamepadPath(string path) => IsAnyDevice(path, Devices.Gamepad, Devices.Dualshock, Devices.XInputController);
    private static bool IsAnyDevice(string path, params Devices[] devices)
    {
        foreach (Devices device in devices) if (IsDevice(path, device)) return true;
        return false;
    }
    private static bool IsDevice(string path, Devices device) => path.StartsWith(GetDeviceName(device));
    private static string GetDeviceName(Devices device) => device switch
    {
        Devices.Gamepad => "<Gamepad>",
        Devices.Dualshock => "<DualShock>",
        Devices.XInputController => "<XInputController>",
        Devices.Mouse => "<Mouse>",
        Devices.Keyboard => "<Keyboard>",
        _ => ""
    };
}