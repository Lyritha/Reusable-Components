using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using System.Linq;

public class ShowButtonPrompt : MonoBehaviour
{
    [SerializeField]
    private PlayerInput input;

    [Header("UI"), SerializeField]
    private TMP_Text text;

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
        if (input == null || input.Action == null) return;
        text.text = string.Join(", ", InputDeviceTracker.GetNamesLastDevice(input.Action, true));
    }
}
