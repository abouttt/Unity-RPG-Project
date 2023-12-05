using System;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;

public class EquipmentInventory : MonoBehaviour
{
    public readonly string SaveKey = "SaveEquipmentInventory";

    private readonly Dictionary<EquipmentType, EquipmentItem> _items = new();
    public event Action<EquipmentType> EquipmentChanged;

    private void Awake()
    {
        var equipmentTypes = Enum.GetValues(typeof(EquipmentType));
        for (int i = 0; i < equipmentTypes.Length; i++)
        {
            _items.Add((EquipmentType)equipmentTypes.GetValue(i), null);
        }

        LoadSaveData();
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

    public JArray GetSaveData()
    {
        var saveDatas = new JArray();

        foreach (var element in _items)
        {
            if (IsNullSlot(element.Key))
            {
                continue;
            }

            ItemSaveData saveData = new()
            {
                ItemID = element.Value.Data.ItemID,
            };

            saveDatas.Add(JObject.FromObject(saveData));
        }

        return saveDatas;
    }

    private void LoadSaveData()
    {
        if (!Managers.Data.Load<JArray>(SaveKey, out var datas))
        {
            return;
        }

        foreach (var data in datas)
        {
            var saveData = data.ToObject<ItemSaveData>();
            EquipItem(ItemDatabase.GetInstance.FindItemBy(saveData.ItemID) as EquipmentItemData);
        }
    }
}
