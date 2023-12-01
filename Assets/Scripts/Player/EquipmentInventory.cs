using System;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class EquipmentInventory : MonoBehaviour
{
    public event Action<EquipmentType> EquipmentChanged;

    private readonly Dictionary<EquipmentType, EquipmentItem> _items = new();

    private void Awake()
    {
        var equipmentTypes = Enum.GetValues(typeof(EquipmentType));
        for (int i = 0; i < equipmentTypes.Length; i++)
        {
            _items.Add((EquipmentType)equipmentTypes.GetValue(i), null);
        }
    }

    public void EquipItem(EquipmentItemData equipmentItemData)
    {
        if (equipmentItemData == null)
        {
            Debug.Log("[EquipmentInventory/EquipItem] equipmentItemData is null");
            return;
        }

        var equipmentItem = equipmentItemData.CreateItem() as EquipmentItem;
        var equipmentType = equipmentItemData.EquipmentType;
        _items[equipmentType]?.Destroy();
        _items[equipmentType] = equipmentItem;
        EquipmentChanged?.Invoke(equipmentType);
    }

    public void UnequipItem(EquipmentType equipmentType)
    {
        _items[equipmentType]?.Destroy();
        _items[equipmentType] = null;
        EquipmentChanged?.Invoke(equipmentType);
    }

    public EquipmentItem GetItem(EquipmentType equipmentType)
    {
        return _items[equipmentType];
    }

    public bool IsNullSlot(EquipmentType equipmentType)
    {
        return _items[equipmentType] is null;
    }
}
