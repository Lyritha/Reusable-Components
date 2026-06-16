using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class TutorialUI : MonoBehaviour
{
    [SerializeField] 
    private CanvasGroup group;
    [SerializeField] 
    private TMP_Text infoText;
    [SerializeField] 
    private RectTransform checkboxParent;
    [SerializeField] 
    private TutorialStepToggle checkboxPrefab;

    private readonly Dictionary<InputAction, TutorialStepToggle> stepToggles = new();

    private void OnEnable()
    {
        TutorialManager.OnStepStarted += CreateNewUI;
        TutorialManager.OnDeviceChanged += UpdateUI;
        TutorialManager.OnActionCompleted += UpdateToggles;
        TutorialManager.OnTutorialCompleted += Complete;
        TutorialManager.OnStepCompleted += DelayBetweenStep;
    }



    private void OnDisable()
    {
        TutorialManager.OnStepStarted -= CreateNewUI;
        TutorialManager.OnDeviceChanged -= UpdateUI;
        TutorialManager.OnActionCompleted -= UpdateToggles;
        TutorialManager.OnTutorialCompleted -= Complete;
        TutorialManager.OnStepCompleted -= DelayBetweenStep;
    }

    private void Start()
    {
        if (group != null)
        {
            group.alpha = 0;
            group.interactable = false;
            group.blocksRaycasts = false;
        }

        StartCoroutine(WaitForSceneLoad());
    }

    private IEnumerator WaitForSceneLoad()
    {
        yield return new WaitUntil(() => SceneManager.GetActiveScene().isLoaded);
        yield return new WaitForSeconds(2f);
        if (TutorialManager.Instance != null)
        {
            if (group != null)
            {
                group.alpha = 1;
                group.interactable = true;
                group.blocksRaycasts = true;
            }

            TutorialManager.Instance.StartTutorial();
        }
    }

    private void DelayBetweenStep()
    {
        StartCoroutine(DelayBetweenStepIn());
    }

    private IEnumerator DelayBetweenStepIn()
    {
        yield return new WaitForSeconds(2f);
        if (TutorialManager.Instance != null) TutorialManager.Instance.NextStep();
    }

    private void CreateNewUI(TutorialStepData data)
    {
        foreach (TutorialStepToggle toggle in stepToggles.Values) Destroy(toggle.gameObject);
        stepToggles.Clear();

        infoText.text = data.Prompt;

        foreach (var (action, actionData) in data.AllActions)
        {
            TutorialStepToggle toggle = Instantiate(checkboxPrefab, checkboxParent);
            stepToggles.Add(action, toggle);
            toggle.Intialize(actionData.Title, actionData.HasBeenCompleted);
            toggle.gameObject.SetActive(data.CurrentDeviceActions.ContainsKey(action));
        }
    }

    private void Complete()
    {
        Destroy(gameObject);
    }

    private void UpdateToggles(InputAction completed)
    {
        if (completed == null) return;
        if (stepToggles.TryGetValue(completed, out TutorialStepToggle toggle)) toggle.Complete();
    }

    private void UpdateUI(TutorialStepData data)
    {
        infoText.text = data.Prompt;

        foreach (var (action, toggle) in stepToggles)
        {
            bool isShown = data.CurrentDeviceActions.ContainsKey(action);
            toggle.gameObject.SetActive(isShown);
            if (isShown && data.CurrentDeviceActions[action].HasBeenCompleted) toggle.Complete();
        }
    }
}