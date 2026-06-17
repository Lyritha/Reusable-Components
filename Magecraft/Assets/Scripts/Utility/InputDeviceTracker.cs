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
    private static readonly Dictionary<(Devices Device, string Name), string> Bindings = new() {
        // Mouse
        { (Devices.Mouse, "delta"), "mouse" },
        { (Devices.Mouse, "left button"), "left mouse" },
        { (Devices.Mouse, "right button"), "right mouse" },
        { (Devices.Mouse, "scroll/up"), "scroll up" },
        { (Devices.Mouse, "scroll/down"), "scroll down" },

        // Controller (shared across Gamepad / DualShock)
        { (Devices.Gamepad, "d-pad/down"), "d-pad down" },
        { (Devices.Gamepad, "d-pad/up"), "d-pad up" },
        { (Devices.Gamepad, "d-pad/left"), "d-pad left" },
        { (Devices.Gamepad, "d-pad/right"), "d-pad right" },
        { (Devices.Gamepad, "button north"), "y" },
        { (Devices.Gamepad, "button east"), "b" },
        { (Devices.Gamepad, "button south"), "a" },
        { (Devices.Gamepad, "button west"), "x" },
    };
    private static readonly Dictionary<Devices, string> DeviceTags = new() {
        { Devices.Gamepad, "<Gamepad>" },
        { Devices.Dualshock, "<DualShock>" },
        { Devices.XInputController, "<XInputController>" },
        { Devices.Mouse, "<Mouse>" },
        { Devices.Keyboard, "<Keyboard>" },
    };
    private static readonly Dictionary<Devices, Devices> FallbackDevice = new() {
        { Devices.XInputController, Devices.Gamepad },
        { Devices.Dualshock, Devices.Gamepad },
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
    public static List<string> GetNamesLastDevice(InputAction action, bool cleanName = false) => GetBindingsLastDevice(action, cleanName).Select(x => x.name).ToList();
    public static List<(string name, string path)> GetBindingsLastDevice(InputAction action, bool cleanName = false)
    {
        bool isGamepad = LastUsedDevice is Gamepad;
        var bindings = action.bindings;

        for (int i = 0; i < bindings.Count; i++)
        {
            InputBinding binding = bindings[i];

            if (binding.isComposite)
            {
                if (i + 1 >= bindings.Count) continue;

                bool isGamepadComposite = IsGamepad(bindings[i + 1].effectivePath);
                if (isGamepad != isGamepadComposite) continue;

                var parts = new List<(string name, string path)>();
                int j = i + 1;
                while (j < bindings.Count && bindings[j].isPartOfComposite)
                {
                    parts.Add(MakeEntry(bindings[j].effectivePath, cleanName));
                    j++;
                }
                return parts;
            }

            if (binding.isPartOfComposite) continue;

            if (isGamepad == IsGamepad(binding.effectivePath))
            {
                return new List<(string name, string path)> { MakeEntry(binding.effectivePath, cleanName) };
            }
        }

        return new List<(string name, string path)> { MakeEntry(action.bindings[0].effectivePath, cleanName) };
    }

    public static List<string> GetAllNames(InputAction action, bool cleanName = false) => GetAllBindings(action, cleanName).Select(x => x.name).ToList();
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
                    result.Add(MakeEntry(bindings[j].effectivePath, cleanName));
                    j++;
                }
                continue;
            }

            if (binding.isPartOfComposite) continue;

            if (!string.IsNullOrEmpty(binding.effectivePath))
                result.Add(MakeEntry(binding.effectivePath, cleanName));
        }

        return result;
    }

    private static (string name, string path) MakeEntry(string path, bool cleanName)
    {
        string rawName = InputControlPath.ToHumanReadableString(path, InputControlPath.HumanReadableStringOptions.OmitDevice);
        return (name: cleanName ? Sanitize(path, rawName) : rawName, path: path);
    }



    // get a binding name that is more human readable
    public static string Sanitize(string path, string name)
    {
        name = name.ToLowerInvariant();

        if (IsDevice(path, Devices.Keyboard)) return name.Length == 1 ? name.ToUpperInvariant() : name;

        Devices? device = GetDevice(path);
        if (device == null) return name;

        if (Bindings.TryGetValue((device.Value, name), out string friendly)) return friendly;
        if (FallbackDevice.TryGetValue(device.Value, out Devices fallback) && Bindings.TryGetValue((fallback, name), out friendly)) return friendly;

        return name;
    }
    private static Devices? GetDevice(string path)
    {
        foreach (Devices device in DeviceTags.Keys) if (IsDevice(path, device)) return device;
        return null;
    }
    private static bool IsDevice(string path, Devices device) => path.StartsWith(DeviceTags[device], StringComparison.Ordinal);
    private static bool IsDeviceGroup(string path, params Devices[] devices)
    {
        foreach (Devices device in devices) if (IsDevice(path, device)) return true;
        return false;
    }
    private static bool IsGamepad(string path) => IsDeviceGroup(path, Devices.Gamepad, Devices.Dualshock, Devices.XInputController);
}