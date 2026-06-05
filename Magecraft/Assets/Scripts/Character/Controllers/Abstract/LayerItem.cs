using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

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