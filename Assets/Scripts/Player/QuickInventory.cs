using System;
using System.Collections.Generic;
using UnityEngine;

public class QuickInventory : MonoBehaviour
{
    [field: SerializeField]
    public int Capacity { get; private set; }

    public event Action<int> InventoryChanged;

    private readonly Dictionary<int, IUsable> _intUsable = new();

    private void Awake()
    {
        for (int i = 0; i < Capacity; i++)
        {
            _intUsable.Add(i, null);
        }
    }

    public void SetUsable(IUsable usable, int index)
    {
        if (usable is null)
        {
            Debug.Log("[QuickInventory/SetUsable] usable is null");
            return;
        }

        _intUsable[index] = usable;
        InventoryChanged?.Invoke(index);
    }

    public void Clear(int index)
    {
        var usable = _intUsable[index];
        if (usable is null)
        {
            return;
        }

        _intUsable[index] = null;
        InventoryChanged?.Invoke(index);
    }

    public void Use(int index)
    {
        var usable = _intUsable[index];
        if (usable is null)
        {
            return;
        }

        usable.Use();
        InventoryChanged?.Invoke(index);
    }

    public IUsable GetUsable(int index)
    {
        return _intUsable[index];
    }

    public void Swap(int indexA, int indexB)
    {
        var usableA = _intUsable[indexA];
        var usableB = _intUsable[indexB];

        SetUsable(usableA, indexB);
        SetUsable(usableB, indexA);
    }
}
