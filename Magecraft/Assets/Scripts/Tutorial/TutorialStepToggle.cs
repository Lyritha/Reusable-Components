using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class TutorialStepToggle : MonoBehaviour
{
    [SerializeField]
    private TMP_Text toggleText;
    [SerializeField]
    private Toggle toggle;

    public void Intialize(InputAction input, bool completed)
    {
        toggleText.text = input.name;
        toggle.isOn = completed;
    }

    public void Complete()
    {
        toggle.isOn = false;
    }
}
