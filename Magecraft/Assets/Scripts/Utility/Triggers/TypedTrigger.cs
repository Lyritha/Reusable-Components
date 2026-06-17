using System;
using System.Collections.Generic;
using UnityEngine;

public class TypedTrigger : TypedTrigger<IInteractable>
{
    protected override void OnTypeEnterTrigger(IInteractable instance) { }
    protected override void OnTypeExitTrigger(IInteractable instance) { }
}


[RequireComponent(typeof(Collider))]
public abstract class TypedTrigger<T> : MonoBehaviour
{
    [SerializeField] private bool allowMultiple = true;

    private T singleInstance;
    private HashSet<T> multiInstances = new();

    public Action<T, bool> OnTriggerChanged;
    public Action<T> OnTriggerEntered;
    public Action<T> OnTriggerExited;

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent(out T instance)) return;

        if (!allowMultiple)
        {
            if (singleInstance != null) return;
            singleInstance = instance;
        }
        else if (!multiInstances.Add(instance)) return;


        OnTriggerChanged?.Invoke(instance, true);
        OnTriggerEntered?.Invoke(instance);

        OnTypeEnterTrigger(instance);
    }

    protected virtual void OnTriggerExit(Collider other)
    {
        if (!other.TryGetComponent(out T instance)) return;

        bool shouldFire = false;

        if (!allowMultiple)
        {
            if (Equals(instance, singleInstance))
            {
                singleInstance = default;
                shouldFire = true;
            }
        }
        else if (multiInstances.Remove(instance)) shouldFire = true;

        if (!shouldFire) return;

        OnTriggerChanged?.Invoke(instance, false);
        OnTriggerExited?.Invoke(instance);

        OnTypeExitTrigger(instance);
    }

    protected abstract void OnTypeEnterTrigger(T instance);
    protected abstract void OnTypeExitTrigger(T instance);
}

