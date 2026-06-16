using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TutorialManager : Singleton<TutorialManager>
{
    [SerializeField] 
    private Tutorial tutorial;

    // step related events, action completed triggers when required action is pressed
    public static event Action<TutorialStepData> OnStepStarted;
    public static event Action<InputAction> OnActionCompleted;
    public static event Action OnStepCompleted;

    // handles updating anything that requires the new device input data (like correct prompt)
    public static event Action<TutorialStepData> OnDeviceChanged;

    // handles when the tutorial is fully completed
    public static event Action OnTutorialCompleted;


    private readonly TutorialStepHandler handler = new();

    private bool hasTutorialStarted = false;
    private bool nextStepRequested = false;
    private bool hasTutorialFinished = false;

    public void StartTutorial() => StartCoroutine(StepHandler());
    public void NextStep() => nextStepRequested = true;

    private void OnEnable()
    {
        handler.OnStepCompleted += HandleActionCompleted;
        handler.OnDeviceChanged += HandleDeviceChanged;
        InputDeviceTracker.OnInputSourceChanged += handler.RefreshForDevice;
    }

    private void OnDisable()
    {
        if (!hasTutorialFinished) Cleanup();
    }

    private IEnumerator StepHandler()
    {
        hasTutorialStarted = true;

        foreach (var step in tutorial.steps)
        {
            if (handler.TrySetNewStep(step, out TutorialStepData data))
                OnStepStarted?.Invoke(data);

            yield return new WaitUntil(() => handler.IsCompleted);

            OnStepCompleted?.Invoke();

            nextStepRequested = false;
            yield return new WaitUntil(() => nextStepRequested);
        }

        CompleteTutorial();
    }

    private void CompleteTutorial()
    {
        Cleanup();
        hasTutorialFinished = true;
        OnTutorialCompleted?.Invoke();
    }

    private void Cleanup()
    {
        handler.OnStepCompleted -= HandleActionCompleted;
        handler.OnDeviceChanged -= HandleDeviceChanged;
        InputDeviceTracker.OnInputSourceChanged -= handler.RefreshForDevice;
        handler.Cleanup();
    }

    private void HandleActionCompleted(InputAction a)
    {
        if (!hasTutorialStarted || hasTutorialFinished) return;
        OnActionCompleted?.Invoke(a);
    }

    private void HandleDeviceChanged(TutorialStepData d)
    {
        if (!hasTutorialStarted || hasTutorialFinished) return;
        OnDeviceChanged?.Invoke(d);
    }
}