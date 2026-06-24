using System;

[Serializable]
public class ItemStack<T>
{
    public T Item;
    public int Count;
    public ItemType Category;

    public event Action<ItemStack<T>> OnChanged;
    public event Action<ItemStack<T>> OnDestroyed;

    public ItemStack(T item, int count, ItemType category)
    {
        Item = item;
        Count = count;
        Category = category;
    }

    public void Add(int amount)
    {
        Count += amount;
        OnChanged?.Invoke(this);
    }

    public void Subtract(int amount)
    {
        Count -= amount;
        OnChanged?.Invoke(this);

        if (Count <= 0) OnDestroyed?.Invoke(this);
    }
}
