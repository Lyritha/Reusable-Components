using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ItemPouch<T> where T : ScriptableObject
{
    public Action<ItemStack<T>> OnStackAdded;
    public Action<ItemStack<T>> OnStackUpdated;
    public Action<T> OnStackRemoved;

    [SerializeField] private List<ItemStack<T>> stacks = new();
    private Dictionary<T, int> indexLookup = new();

    public void AddItems(T item, int quantity)
    {
        if (indexLookup.TryGetValue(item, out int index))
        {
            ItemStack<T> stack = stacks[index];
            stack.Count += quantity;

            OnStackUpdated?.Invoke(stack);
        }
        else
        {
            ItemStack<T> newStack = new(item, quantity);
            stacks.Add(newStack);

            int newIndex = stacks.Count - 1;
            indexLookup[item] = newIndex;

            OnStackAdded?.Invoke(newStack);
        }
    }

    public int Consume(T item, int requested)
    {
        if (!indexLookup.TryGetValue(item, out int index)) return 0;

        ItemStack<T> stack = stacks[index];
        int available = stack.Count;

        if (available <= 0) return 0;

        int consumed = Mathf.Min(requested, available);
        int remaining = available - consumed;

        if (remaining <= 0)
        {
            stacks.RemoveAt(index);
            indexLookup.Remove(item);

            // Fix shifted indexes
            for (int i = index; i < stacks.Count; i++)
            {
                T shiftedItem = stacks[i].Item;
                indexLookup[shiftedItem] = i;
            }

            OnStackRemoved?.Invoke(item);
        }
        else
        {
            stack.Count = remaining;
            OnStackUpdated?.Invoke(stack);
        }

        return consumed;
    }

    public int GetCount(T item)
    {
        int index;
        if (!indexLookup.TryGetValue(item, out index))
            return 0;

        return stacks[index].Count;
    }

    public IEnumerable<ItemStack<T>> AllStacks => stacks;
}