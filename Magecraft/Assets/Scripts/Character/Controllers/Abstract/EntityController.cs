using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class EntityController : SingletonGroup<EntityController>
{
    public event Action OnControllerDestroyed;

    public Layer ActiveLayer { get; private set; } = Layer.Movement;

// enclosed in pragma to avoid "unused" warnings for entities that don't use certain events
#pragma warning disable CS0067

    public LayerItem<Vector2> Move = new() { layers = {Layer.Movement } };
    public LayerItem<Vector2> LookDelta = new() { layers = {Layer.Movement } };

    public LayerItem<bool> Sprint = new() { layers = {Layer.Movement } };
    public LayerItem<bool> PrimaryMouse = new() { layers = {Layer.Movement, Layer.UI } };
    public LayerItem<bool> SecondaryMouse = new() { layers = {Layer.Movement, Layer.UI } };

    public LayerItem<int> NumberSelected = new() { layers = { Layer.Movement } };

    public LayerItem Jump = new() { layers = {Layer.Movement } };
    public LayerItem Interact = new() { layers = {Layer.Movement } };
    public LayerItem Tab = new() { layers = {Layer.Movement } };
    public LayerItem Inventory = new() { layers = {Layer.Movement, Layer.UI } };

#pragma warning restore CS0067

    public void SetActiveLayer(Layer layer)
    {
        Move.Raise(Vector2.zero, ActiveLayer);
        LookDelta.Raise(Vector2.zero, ActiveLayer);

        Sprint.Raise(false, ActiveLayer);
        PrimaryMouse.Raise(false, ActiveLayer);
        SecondaryMouse.Raise(false, ActiveLayer);

        NumberSelected.Raise(0, ActiveLayer);

        ActiveLayer = layer;
    }

    protected override void OnDestroy()
    {
        OnControllerDestroyed?.Invoke();
        base.OnDestroy();
    }
}