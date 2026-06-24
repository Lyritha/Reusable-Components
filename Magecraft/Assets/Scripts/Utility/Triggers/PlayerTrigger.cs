using System;
using UnityEngine;
using UnityEngine.Events;

public class PlayerTrigger : TypedTrigger<PlayerController>
{
    public UnityEvent OnPlayerEnter;
    public UnityEvent OnPlayerExit;

    private void Awake()
    {
        OnPlayerExit?.Invoke();
    }

    protected override void OnTypeEnterTrigger(PlayerController instance)
    {
        OnPlayerEnter?.Invoke();
    }

    protected override void OnTypeExitTrigger(PlayerController instance)
    {
        OnPlayerExit?.Invoke();
    }
}
