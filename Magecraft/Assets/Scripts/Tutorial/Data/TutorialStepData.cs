using System.Collections.Generic;
using UnityEngine.InputSystem;

public struct TutorialStepData
{
    public string Prompt;
    public Dictionary<InputAction, InputActionData> AllActions;
    public Dictionary<InputAction, InputActionData> CurrentDeviceActions;
}
