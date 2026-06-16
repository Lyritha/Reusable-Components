using System;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "Tutorial Step", menuName = "Tutorial/Tutorial Step")]
public class TutorialStep : ScriptableObject
{
    public InputActionReference input;
    [Tooltip("Use <Input> where it should show the inputs")]
    public string prompt;
}
