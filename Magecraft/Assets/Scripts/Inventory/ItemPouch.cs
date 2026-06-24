using System;
using System.Collections.Generic;
using UnityEngine;

public class ItemPouch<T> where T : class
{
    public Action<ItemStack<T>> OnStackAdded;
    public Action<ItemStack<T>> OnStackRemoved;

    public List<ItemStack<T>> Stacks { get; private set; } = new();
    private Dictionary<T, int> indexLookup = new();

    public void AddItems(T item, int quantity, ItemType type)
    {
        if (indexLookup.TryGetValue(item, out int index)) Stacks[index].Add(quantity);
        else
        {
            ItemStack<T> stack = new(item, quantity, type);
            Stacks.Add(stack);
            indexLookup[item] = Stacks.Count - 1;

            stack.OnDestroyed += HandleStackDestroyed;
            OnStackAdded?.Invoke(stack);
        }
    }

    private void HandleStackDestroyed(ItemStack<T> stack)
    {
        OnStackRemoved?.Invoke(stack);
        int index = indexLookup[stack.Item];

        Stacks.RemoveAt(index);
        indexLookup.Remove(stack.Item);

        // fix shifted indexes
        for (int i = index; i < Stacks.Count; i++) indexLookup[Stacks[i].Item] = i;
    }

    public int Consume(T item, int requested)
    {
        if (!indexLookup.TryGetValue(item, out int index)) return 0;

        ItemStack<T> stack = Stacks[index];
        int consumed = Mathf.Min(requested, stack.Count);

        stack.Subtract(consumed);
        return consumed;
    }
}
