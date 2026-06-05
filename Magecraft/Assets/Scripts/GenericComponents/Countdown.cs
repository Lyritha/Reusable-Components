using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class Countdown : MonoBehaviour
{
    [SerializeField]
    private float duration = 5f;

    public float TimeLeft { get; private set; }
    public UnityEvent OnCountdownStarted = new();
    public UnityEvent<float> OnCountdownProgress = new();
    public UnityEvent OnCountdownEnded = new();

    private Coroutine countdownRoutine;

    public void StartCountdown(float duration, bool reset = false)
    {
        this.duration = duration;
        StartCountdown(reset);
    }

    public void StartCountdown(bool reset = false)
    {
        bool isRunning = countdownRoutine != null;

        if (reset)
        {
            if (isRunning) StopCoroutine(countdownRoutine);
            countdownRoutine = StartCoroutine(CountdownRoutine());

            return;
        }

        if (!isRunning)
        {
            countdownRoutine = StartCoroutine(CountdownRoutine());
        }
    }

    private IEnumerator CountdownRoutine()
    {
        OnCountdownStarted?.Invoke();

        TimeLeft = duration;

        while (TimeLeft > 0f)
        {
            TimeLeft -= Time.deltaTime;
            OnCountdownProgress?.Invoke(TimeLeft / duration);
            yield return null;
        }

        TimeLeft = 0f;
        OnCountdownEnded?.Invoke();
    }
}
