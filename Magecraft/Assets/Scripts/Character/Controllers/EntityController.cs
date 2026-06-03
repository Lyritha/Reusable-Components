using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

public class EntityController : IdentifiableBehaviour<EntityController>
{
    public Layer ActiveLayer = Layer.Movement;

// enclosed in pragma to avoid "unused" warnings for entities that don't use certain events
#pragma warning disable CS0067

    public LayerItem<Vector2> OnMove = new() { layers = {Layer.Movement } };
    public LayerItem<Vector2> OnLookDelta = new() { layers = {Layer.Movement } };

    public LayerItem<bool> OnSprint = new() { layers = {Layer.Movement } };
    public LayerItem OnJump = new() { layers = {Layer.Movement } };

    public LayerItem<bool> OnPrimaryMouse = new() { layers = {Layer.Movement, Layer.UI } };
    public LayerItem<bool> OnSecondaryMouse = new() { layers = {Layer.Movement, Layer.UI } };
    public LayerItem OnInteract = new() { layers = {Layer.Movement } };

    public LayerItem OnTab = new() { layers = {Layer.Movement } };
    public LayerItem OnInventory = new() { layers = {Layer.Movement, Layer.UI } };

    public LayerItem<int> OnNumberSelected = new() { layers = { Layer.Movement, Layer.UI } };

#pragma warning restore CS0067


    public event Action OnControllerDestroyed;
    protected override void OnDestroy()
    {
        OnControllerDestroyed?.Invoke();
        base.OnDestroy();
    }
}

public enum Layer
{
    Movement,
    UI
}

public class LayerItem<T>
{
    public List<Layer> layers = new() { Layer.Movement };

    public event Action<T> OnEvent;
    public void Raise(T v, Layer currentLayer)
    {
        if (!layers.Contains(currentLayer)) return;
        OnEvent?.Invoke(v);
    }
}
public class LayerItem
{
    public List<Layer> layers = new() { Layer.Movement };

    public event Action OnEvent;
    public void Raise(Layer currentLayer)
    {
        if (!layers.Contains(currentLayer)) return;
        OnEvent?.Invoke();
    }
}