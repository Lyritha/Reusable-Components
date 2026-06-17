using System;
using UnityEngine;
using UnityEngine.Events;

public class PlayerTrigger : TypedTrigger<PlayerController>
{
    public UnityEvent<int> OnPlayerEnter;
    public UnityEvent<int> OnPlayerExit;

    protected override void OnTypeEnterTrigger(PlayerController instance)
    {
        OnPlayerEnter?.Invoke((int)instance.InstanceId);
    }

    protected override void OnTypeExitTrigger(PlayerController instance)
    {
        OnPlayerExit?.Invoke((int)instance.InstanceId);
    }
}
