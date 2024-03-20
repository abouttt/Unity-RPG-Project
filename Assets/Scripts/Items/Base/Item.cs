using System;
using UnityEngine;

public abstract class Item
{
    public event Action ItemChanged;
    public ItemData Data { get; private set; }
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
