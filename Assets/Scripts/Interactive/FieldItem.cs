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
    private int _currentCount;

    private void Awake()
    {
        _currentCount = _items.Count;
    }

    private void Start()
    {
        if (_items == null || _items.Count == 0)
        {
            Managers.Resource.Destroy(gameObject);
        }
        else
        {
            InstantiateMinimapIcon("ItemMinimapIcon.sprite", "æ∆¿Ã≈€");
        }
    }

    public override void Interaction()
    {
        Managers.UI.Show<UI_LootPopup>().SetFieldItem(this);
    }

    public void AddItem(ItemData item, int count)
    {
        var data = new Data()
        {
            ItemData = item,
            Count = count
        };

        _items.Add(data);
        _currentCount++;
    }

    public void RemoveItem(int index)
    {
        if (_items[index] == null)
        {
            return;
        }

        _items[index] = null;
        _currentCount--;

        if (_currentCount == 0)
        {
            Managers.Resource.Destroy(gameObject);
        }
    }
}
