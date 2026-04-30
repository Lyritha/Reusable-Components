using System;
using System.Collections.Generic;
using UnityEngine;

public class InventoryPouch<T> where T : ScriptableObject
{
    public Action<Dictionary<T, int>> OnPouchUpdated;
    private Dictionary<T, int> items = new();

    public void AddItems(T item, int quantity = 1)
    {
        if (items.ContainsKey(item)) items[item] += quantity;
        else items[item] = quantity;

        OnPouchUpdated?.Invoke(items);
    }

    public int Consume(T item, int requested)
    {
        if (!items.TryGetValue(item, out int available) || available <= 0)
            return 0;

        int consumed = Mathf.Min(requested, available);

        available -= consumed;

        if (available <= 0) items.Remove(item);
        else items[item] = available;

        OnPouchUpdated?.Invoke(items);
        return consumed;
    }


    public Dictionary<T, int> Items => items;
    public int GetCount(T item) => items.TryGetValue(item, out int count) ? count : 0;
}
