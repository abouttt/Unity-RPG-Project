using System;
using UnityEngine;

public abstract class Item
{
    public ItemData Data { get; private set; }
    public event Action ItemChanged;
    public bool IsDestroyed { get; private set; } = false;

    public Item(ItemData data)
    {
        Data = data;
    }

    public void Destroy()
    {
        if (IsDestroyed)
        {
            return;
        }

        IsDestroyed = true;
        ItemChanged?.Invoke();
        ItemChanged = null;
    }

    protected void OnItemChanged()
    {
        ItemChanged?.Invoke();
    }
}
