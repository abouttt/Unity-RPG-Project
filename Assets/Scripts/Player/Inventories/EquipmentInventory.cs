using System;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;

public class EquipmentInventory : MonoBehaviour, ISavable
{
    public static string SaveKey => "SaveEquipmentInventory";

    public event Action<EquipmentType> InventoryChanged;

    private readonly Dictionary<EquipmentType, EquipmentItem> _items = new();

    private void Awake()
    {
        var types = Enum.GetValues(typeof(EquipmentType));
        for (int i = 0; i < types.Length; i++)
        {
            _items.Add((EquipmentType)types.GetValue(i), null);
        }

        LoadSaveData();
    }

    public void Equip(EquipmentItemData equipmentItemData)
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
        InventoryChanged?.Invoke(equipmentType);
    }

    public void Unequip(EquipmentType equipmentType)
    {
        _items[equipmentType]?.Destroy();
        _items[equipmentType] = null;
        InventoryChanged?.Invoke(equipmentType);
    }

    public EquipmentItem GetItem(EquipmentType equipmentType)
    {
        return _items[equipmentType];
    }

    public bool IsEquipped(EquipmentType equipmentType)
    {
        return _items[equipmentType] != null;
    }

    public JArray CreateSaveData()
    {
        var saveData = new JArray();

        foreach (var element in _items)
        {
            if (!IsEquipped(element.Key))
            {
                continue;
            }

            var itemSaveData = new ItemSaveData()
            {
                ItemID = element.Value.Data.ItemID,
            };

            saveData.Add(JObject.FromObject(itemSaveData));
        }

        return saveData;
    }

    public void LoadSaveData()
    {
        if (!Managers.Data.Load<JArray>(SaveKey, out var saveData))
        {
            return;
        }

        foreach (var token in saveData)
        {
            var itemSaveData = token.ToObject<ItemSaveData>();
            Equip(ItemDatabase.GetInstance.FindItemBy(itemSaveData.ItemID) as EquipmentItemData);
        }
    }

    private void OnDestroy()
    {
        InventoryChanged = null;
    }
}
