using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using System.Linq;

public class ShowButtonPrompt : MonoBehaviour
{
    [Header("Input Action")]
    public InputActionReference action;

    [Header("UI")]
    public TMP_Text text;

    private void OnEnable()
    {
        UpdatePrompt();
        InputDeviceTracker.OnInputSourceChanged += UpdatePrompt;
    }

    private void OnDisable()
    {
        InputDeviceTracker.OnInputSourceChanged -= UpdatePrompt;
    }

    private void UpdatePrompt()
    {
        if (action == null || action.action == null) return;
        text.text = string.Join(", ", InputDeviceTracker.GetNamesLastDevice(action, true));
    }
}
