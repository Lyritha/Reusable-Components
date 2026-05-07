using System;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public abstract class TypedTrigger<T> : MonoBehaviour where T : MonoBehaviour
{
    [SerializeField]
    private bool allowMultiple = true;
    private T currentComponent;

    /// <summary>
    /// Triggers when a Monobehavior of <typeparamref name="T"/> Enters or exits the trigger
    /// </summary>
    public Action<T, bool> OnTriggerChanged;
    public Action<T> OnTriggerEntered;
    public Action<T> OnTriggerExited;

    protected virtual void OnTriggerEnter(Collider other)
    {
        // if only one T is allowed, return
        if (!allowMultiple && currentComponent != null) return;

        if (other.TryGetComponent(out T targetComp))
        {
            OnTriggerChanged?.Invoke(targetComp, true);
            OnTriggerEntered?.Invoke(targetComp);
            currentComponent = targetComp;

            OnTypeEnterTrigger();
        }
    }
    protected abstract void OnTypeEnterTrigger();


    protected virtual void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out T targetComp) && targetComp == currentComponent)
        {
            OnTriggerChanged?.Invoke(targetComp, false);
            OnTriggerExited?.Invoke(targetComp);
            currentComponent = null;

            OnTypeExitTrigger();
        }
    }
    protected abstract void OnTypeExitTrigger();
}

