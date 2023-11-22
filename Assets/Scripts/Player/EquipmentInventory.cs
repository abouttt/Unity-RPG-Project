using System;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentInventory : MonoBehaviour
{
    public event Action<EquipmentType> Changed;

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
        _items[equipmentItemData.EquipmentType]?.Destroy();
        ChangeItem(equipmentItemData.EquipmentType, equipmentItem);
    }

    public void UnequipItem(EquipmentType equipmentType)
    {
        _items[equipmentType]?.Destroy();
        ChangeItem(equipmentType, null);
    }

    public EquipmentItem GetItem(EquipmentType equipmentType)
    {
        return _items[equipmentType];
    }

    public bool IsNullSlot(EquipmentType equipmentType)
    {
        return _items[equipmentType] is null;
    }

    private void ChangeItem(EquipmentType equipmentType, EquipmentItem item)
    {
        _items[equipmentType] = item;
        Changed?.Invoke(equipmentType);
    }
}
