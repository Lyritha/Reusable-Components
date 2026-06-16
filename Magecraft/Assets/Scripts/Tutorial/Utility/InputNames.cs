using System.Collections.Generic;
using UnityEngine;

public static class InputNames
{
    private static readonly Dictionary<string, string> MouseBindings = new()
    {
        { "delta", "mouse" },
        { "left button", "left mouse" },
        { "right button", "right mouse" },
        { "scroll/up", "scroll up" },
        { "scroll/down", "scroll down" },
    };

    private static readonly Dictionary<string, string> ControllerBindings = new()
    {
        { "d-pad/down", "d-pad down" },
        { "d-pad/up", "d-pad up" },
        { "d-pad/left", "d-pad left" },
        { "d-pad/right", "d-pad right" },
    };

    public static string GetCleanBindingName(string path, string name)
    {
        name = name.ToLower();

        if (path.StartsWith("<Mouse>")) return MouseBindings.TryGetValue(name, out string friendly) ? friendly : name;
        if (path.StartsWith("<Keyboard>")) return name;
        if (path.StartsWith("<Gamepad>") || path.StartsWith("<XInputController>") || path.StartsWith("<DualShock>")) return ControllerBindings.TryGetValue(name, out string friendly) ? friendly : name;

        return name;
    }
}
