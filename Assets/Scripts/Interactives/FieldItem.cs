using System;
using System.Collections.Generic;
using UnityEngine;

public class FieldItem : Interactive
{
    [Serializable]
    public class Data
    {
        public ItemData ItemData;
        public int Count;
    }

    public IReadOnlyList<Data> Items => _items;

    [SerializeField]
    private List<Data> _items;
    private int _count;

    protected override void Awake()
    {
        if (_items == null || _items.Count == 0)
        {
            Managers.Resource.Destroy(gameObject);
            return;
        }

        base.Awake();
        _count = _items.Count;
    }

    public override void Interaction()
    {
        Managers.UI.Show<UI_LootPopup>().SetFieldItem(this);
    }

    public void AddItem(ItemData item, int count)
    {
        var data = new Data
        {
            ItemData = item,
            Count = count
        };

        _items.Add(data);
        _count++;
    }

    public void RemoveItem(int index)
    {
        if (_items[index] is null)
        {
            return;
        }

        _items[index] = null;
        _count--;

        if (_count == 0)
        {
            Managers.Resource.Destroy(gameObject);
        }
    }
}
