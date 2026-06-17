using System;
using System.Collections.Generic;

public class LayerItem<T>
{
    public List<Layer> layers = new() { Layer.Movement };

    private LayerItem<T> source;

    public event Action<T> OnEvent;

    public void SetSource(LayerItem<T> newSource)
    {
        if (source != null) source.OnEvent -= Relay;
        source = newSource;

        if (source != null) source.OnEvent += Relay;
    }

    private void Relay(T v) => OnEvent?.Invoke(v);

    public void Raise(T v, Layer currentLayer)
    {
        if (!layers.Contains(currentLayer)) return;
        OnEvent?.Invoke(v);
    }
}

public class LayerItem
{
    public List<Layer> layers = new() { Layer.Movement };

    private LayerItem source;

    public event Action OnEvent;

    public void SetSource(LayerItem newSource)
    {
        if (source != null) source.OnEvent -= Relay;
        source = newSource;

        if (source != null) source.OnEvent += Relay;
    }

    private void Relay() => OnEvent?.Invoke();

    public void Raise(Layer currentLayer)
    {
        if (!layers.Contains(currentLayer)) return;
        OnEvent?.Invoke();
    }
}
