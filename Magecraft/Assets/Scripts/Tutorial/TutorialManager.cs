using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    [SerializeField]
    private List<TutorialStep> steps;
    [SerializeField]
    private TMP_Text infoText;
    [SerializeField]
    private RectTransform checkboxParent;
    [SerializeField]
    private TutorialStepToggle checkboxPrefab;

    private TutorialStep activeStep;


    private void Start()
    {
        foreach (var step in steps) step.Initialize();
        StartCoroutine(StepHandler());
    }

    private void OnEnable() => InputDeviceTracker.OnInputSourceChanged += UpdateUI;
    private void OnDisable() => InputDeviceTracker.OnInputSourceChanged -= UpdateUI;

    private void UpdateUI()
    {
        if (activeStep == null) return;

        infoText.text = activeStep.GetPrompt();
        foreach (Transform child in checkboxParent) Destroy(child.gameObject);

        foreach (KeyValuePair<InputAction, bool> input in activeStep.ActiveRequiredActions)
        {
            TutorialStepToggle toggle = Instantiate(checkboxPrefab, checkboxParent);
            toggle.Intialize(input.Key, input.Value);
        }
    }

    private IEnumerator StepHandler()
    {
        foreach (var step in steps)
        {
            activeStep = step;
            UpdateUI();

            activeStep.InputCompleted += UpdateUI;
            yield return new WaitUntil(() => activeStep.IsCompleted);
            activeStep.InputCompleted -= UpdateUI;

        }

        Debug.Log("finished tutorial");
    }

}
