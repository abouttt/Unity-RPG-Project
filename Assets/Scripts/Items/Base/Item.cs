using System;
using UnityEngine;

public abstract class Item
{
    public ItemData Data { get; private set; }
    public int Index { get; set; } = -1;
    public bool IsDestroyed { get; private set; } = false;
    public event Action ItemChanged;

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
        Index = -1;
        ItemChanged = null;
    }

    protected void OnItemChanged()
    {
        ItemChanged?.Invoke();
    }
}
