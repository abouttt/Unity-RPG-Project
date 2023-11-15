using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using AYellowpaper.SerializedCollections;

public class ItemInventory : MonoBehaviour
{
    [Serializable]
    public class Inventory
    {
        public List<Item> Items;
        public int Capacity;
        [ReadOnly]
        public int Count;
    }

    public IReadOnlyDictionary<ItemType, Inventory> Inventories => _inventories;
    public event Action<ItemType, int> InventoryChanged;

    [SerializeField]
    private SerializedDictionary<ItemType, Inventory> _inventories;

    private void Awake()
    {
        foreach (var element in _inventories)
        {
            var inventory = element.Value;
            inventory.Items = Enumerable.Repeat<Item>(null, inventory.Capacity).ToList();
        }
    }

    public int AddItem(ItemData itemData, int count = 1)
    {
        Debug.Assert(count > 0, "[ItemInventory/AddItem] count must be > 0");

        if (itemData == null)
        {
            Debug.Log("[ItemInventory/AddItem] itemData is null");
            return count;
        }

        var inventory = _inventories[itemData.ItemType];
        var countableItemData = itemData as CountableItemData;

        while (count > 0)
        {
            if (countableItemData != null)
            {
                if (TryGetSameNoMaxCountalbeItemIndex(inventory, countableItemData, out var sameIndex))
                {
                    var otherItem = inventory.Items[sameIndex] as CountableItem;
                    int prevCount = count;
                    count = otherItem.AddCountAndGetExcess(count);
                    InventoryChanged?.Invoke(itemData.ItemType, sameIndex);
                }
                else
                {
                    if (TryGetEmptyIndex(itemData.ItemType, out var emptyIndex))
                    {
                        SetItem(countableItemData, emptyIndex, count);
                        count = Mathf.Clamp(count - countableItemData.MaxCount, 0, countableItemData.MaxCount);
                    }
                    else
                    {
                        break;
                    }
                }
            }
            else
            {
                if (TryGetEmptyIndex(itemData.ItemType, out var emptyIndex))
                {
                    SetItem(itemData, emptyIndex, count);
                    count--;
                }
                else
                {
                    break;
                }
            }
        }

        return count;
    }

    public void RemoveItem(ItemType itemType, int index)
    {
        if (IsNullSlot(itemType, index))
        {
            return;
        }

        DestroyItem(itemType, index);
        InventoryChanged?.Invoke(itemType, index);
    }

    public void RemoveItem(string id, int count)
    {
        ItemType itemType = GetItemTypeByID(id);

        for (int index = 0; index < _inventories[itemType].Items.Count; index++)
        {
            var item = _inventories[itemType].Items[index];

            if (item is null)
            {
                continue;
            }

            if (!item.Data.ItemID.Equals(id))
            {
                continue;
            }

            if (item is CountableItem countableItem)
            {
                if (countableItem.CurrentCount > count)
                {
                    countableItem.SetCount(countableItem.CurrentCount - count);
                    InventoryChanged?.Invoke(itemType, index);
                    break;
                }
                else
                {
                    count -= countableItem.CurrentCount;
                }
            }
            else
            {
                count--;
            }

            RemoveItem(itemType, index);

            if (count == 0)
            {
                break;
            }
        }
    }

    public void MoveItem(ItemType itemType, int fromIndex, int toIndex)
    {
        if (fromIndex == toIndex)
        {
            return;
        }

        if (!AddItemCountFromTo(itemType, fromIndex, toIndex))
        {
            SwapItem(itemType, fromIndex, toIndex);
        }

        InventoryChanged?.Invoke(itemType, fromIndex);
        InventoryChanged?.Invoke(itemType, toIndex);
    }

    public void SplitItem(ItemType itemType, int fromIndex, int toIndex, int count)
    {
        Debug.Assert(count > 0, "[ItemInventory/SplitItem] count must be > 0");

        if (fromIndex == toIndex)
        {
            return;
        }

        if (IsNullSlot(itemType, fromIndex) && !IsNullSlot(itemType, toIndex))
        {
            return;
        }

        var inventory = _inventories[itemType];
        if (inventory.Items[fromIndex] is not CountableItem countableItem)
        {
            return;
        }

        int nextCount = countableItem.CurrentCount - count;
        if (nextCount < 0)
        {
            return;
        }
        else if (nextCount == 0)
        {
            SwapItem(itemType, fromIndex, toIndex);
        }
        else
        {
            countableItem.SetCount(nextCount);
            inventory.Items[toIndex] = countableItem.CountableData.CreateItem(count);
            inventory.Items[toIndex].Index = toIndex;
            inventory.Count++;
        }

        InventoryChanged?.Invoke(itemType, fromIndex);
        InventoryChanged?.Invoke(itemType, toIndex);
    }

    public void SetItem(ItemData itemData, int index, int count = 1)
    {
        Debug.Assert(count > 0, "[ItemInventory/SetItem] count must be > 0");

        if (itemData == null)
        {
            Debug.Log("[ItemInventory/SetItem] itemData is null");
            return;
        }

        var inventory = _inventories[itemData.ItemType];

        if (IsNullSlot(itemData.ItemType, index))
        {
            inventory.Count++;
        }
        else
        {
            DestroyItem(itemData.ItemType, index);
        }

        if (itemData is CountableItemData countableItemData)
        {
            inventory.Items[index] = countableItemData.CreateItem(count);
        }
        else
        {
            inventory.Items[index] = itemData.CreateItem();
            count = 1;
        }

        inventory.Items[index].Index = index;
        InventoryChanged?.Invoke(itemData.ItemType, index);
    }

    public T GetItem<T>(ItemType itemType, int index) where T : Item
    {
        return _inventories[itemType].Items[index] as T;
    }

    public int GetSameItemCount(string id)
    {
        ItemType itemType = GetItemTypeByID(id);

        int count = 0;
        foreach (var item in _inventories[itemType].Items)
        {
            if (item is null)
            {
                continue;
            }

            if (!item.Data.ItemID.Equals(id))
            {
                continue;
            }

            if (item is CountableItem countableItem)
            {
                count += countableItem.CurrentCount;
            }
            else
            {
                count++;
            }
        }

        return count;
    }

    public bool IsNullSlot(ItemType itemType, int index)
    {
        return _inventories[itemType].Items[index] is null;
    }

    private bool AddItemCountFromTo(ItemType itemType, int fromIndex, int toIndex)
    {
        if (IsNullSlot(itemType, fromIndex) || IsNullSlot(itemType, toIndex))
        {
            return false;
        }

        var inventory = _inventories[itemType];

        if (inventory.Items[fromIndex] is not CountableItem fromItem ||
            inventory.Items[toIndex] is not CountableItem toItem)
        {
            return false;
        }

        if (!fromItem.Data.Equals(toItem.Data))
        {
            return false;
        }

        if (toItem.IsMax)
        {
            return false;
        }

        int excess = toItem.AddCountAndGetExcess(fromItem.CurrentCount);
        fromItem.SetCount(excess);
        if (fromItem.IsEmpty)
        {
            DestroyItem(itemType, fromIndex);
        }

        return true;
    }

    private void DestroyItem(ItemType itemType, int index)
    {
        var inventory = _inventories[itemType];
        var item = inventory.Items[index];
        int count = 1;
        if (item is CountableItem countableItem)
        {
            count = countableItem.CurrentCount;
        }
        item.Destroy();
        inventory.Items[index] = null;
        inventory.Count--;
    }

    private bool TryGetEmptyIndex(ItemType itemType, out int index)
    {
        index = _inventories[itemType].Items.FindIndex(item => item is null);
        return index != -1;
    }

    private void SwapItem(ItemType itemType, int Aindex, int BIndex)
    {
        var inventory = _inventories[itemType];

        if (!IsNullSlot(itemType, Aindex))
        {
            inventory.Items[Aindex].Index = BIndex;
        }

        if (!IsNullSlot(itemType, BIndex))
        {
            inventory.Items[BIndex].Index = Aindex;
        }

        (inventory.Items[Aindex], inventory.Items[BIndex]) = (inventory.Items[BIndex], inventory.Items[Aindex]);
    }

    private bool TryGetSameNoMaxCountalbeItemIndex(Inventory inventory, CountableItemData countableItemData, out int index)
    {
        index = inventory.Items.FindIndex(item =>
        {
            if (item is null)
            {
                return false;
            }

            if (!item.Data.Equals(countableItemData))
            {
                return false;
            }

            var countableItem = item as CountableItem;
            return !countableItem.IsMax;
        });

        return index != -1;
    }

    private Type GetItemDataByItemType(ItemType type)
    {
        return type switch
        {
            ItemType.Equipment => typeof(EquipmentItemData),
            ItemType.Consumable => typeof(ConsumableItemData),
            ItemType.Etc => typeof(EtcItemData),
            _ => throw new NotImplementedException(),
        };
    }

    private ItemType GetItemTypeByID(string id)
    {
        return id[..id.IndexOf('_')] switch
        {
            "EQUIPMENT" => ItemType.Equipment,
            "CONSUMABLE" => ItemType.Consumable,
            "ETC" => ItemType.Etc,
            _ => throw new NotImplementedException(),
        };
    }

    private void OnDestroy()
    {
        foreach (var element in _inventories)
        {
            var itemDataType = GetItemDataByItemType(element.Key);
            if (typeof(ICooldownable).IsAssignableFrom(itemDataType))
            {
                foreach (var item in element.Value.Items)
                {
                    if (item is null)
                    {
                        continue;
                    }

                    if (item.Data is ICooldownable cooldownable)
                    {
                        cooldownable.Cooldown.Clear();
                    }
                }
            }
        }
    }
}
