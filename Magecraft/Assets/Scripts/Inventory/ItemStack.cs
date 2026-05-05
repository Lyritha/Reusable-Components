using System;
using UnityEngine;

[Serializable]
public class ItemStack<T>
{
    public T Item;
    public int Count;

    public ItemStack(T item, int count)
    {
        Item = item;
        Count = count;
    }
}
