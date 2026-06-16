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

    public void Intialize(string title, bool completed)
    {
        toggleText.text = title;
        toggle.isOn = completed;
    }

    public void Complete()
    {
        toggle.isOn = true;
    }
}
