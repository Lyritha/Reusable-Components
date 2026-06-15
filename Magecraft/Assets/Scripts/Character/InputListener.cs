using System;
using System.Collections.Generic;
using UnityEngine;
using Lyrith.Utility.Fold;

/// <summary>
/// Base class for components that subscribe to events on an EntityController.
/// Must be on the same gameobject or a child object of an EntityController.
/// <list type="bullet">
/// <item>Finds the EntityController in parent objects</item>
/// <item>Subscribes automatically when the EntityController becomes available</item>
/// <item>Unsubscribes automatically on disable</item>
/// <item>Handles late subscriptions safely</item>
/// </list>
/// </summary>
[Fold]
public abstract class InputListener : MonoBehaviour
{
    [SerializeField, ShowOnly]
    protected EntityController entity;

    private readonly List<(Action<EntityController> sub, Action<EntityController> unsub)> subs = new();

    private float retryTimer = 0f;
    private const float RetryInterval = 0.25f;

    protected virtual void Start() => TryFindEntity();

    protected virtual void Update()
    {
        if (entity == null)
        {
            retryTimer -= Time.deltaTime;
            if (retryTimer <= 0f)
            {
                retryTimer = RetryInterval;
                TryFindEntity();
            }
        }
    }

    private void TryFindEntity()
    {
        EntityController found = GetComponentInParent<EntityController>();

        if (found == null) return;

        if (entity != found)
        {
            if (entity != null) RemoveAllSubscriptions();

            entity = found;
            ApplySubscriptions();
        }
    }

    private void ApplySubscriptions()
    {
        foreach (var (sub, unsub) in subs) sub(entity);
    }

    /// <summary>
    /// Registers a subscription pair for an EntityController event.
    /// Example:
    /// <code>
    /// AddSubscription(
    ///     ec => ec.OnMove += HandleMove,
    ///     ec => ec.OnMove -= HandleMove
    /// );
    /// </code>
    /// </summary>
    protected void AddSubscription(Action<EntityController> subscribe, Action<EntityController> unsubscribe)
    {
        subs.Add((subscribe, unsubscribe));
        if (entity != null) subscribe(entity);
    }

    /// <summary>
    /// Removes a previously added subscription pair (expressions must be the same for it to succeed).
    /// </summary>
    protected void RemoveSubscription(Action<EntityController> subscribe, Action<EntityController> unsubscribe)
    {
        var pair = (subscribe, unsubscribe);
        if (subs.Remove(pair) && entity != null) unsubscribe(entity);
    }

    private void OnEnable()
    {
        if (entity != null) ApplySubscriptions();
        else retryTimer = 0f;
    }

    protected virtual void OnDisable() => RemoveAllSubscriptions();

    private void RemoveAllSubscriptions()
    {
        if (entity == null) return;
        foreach (var (sub, unsub) in subs) unsub(entity);
    }

    private void OnDestroy()
    {
        RemoveAllSubscriptions();

    }
}
