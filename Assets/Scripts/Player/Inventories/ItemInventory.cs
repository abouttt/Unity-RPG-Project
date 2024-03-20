using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Newtonsoft.Json.Linq;
using AYellowpaper.SerializedCollections;

public class ItemInventory : MonoBehaviour, ISavable
{
    [Serializable]
    public class Inventory
    {
        public List<Item> Items;
        public int Capacity;
        [ReadOnly]
        public int Count;
    }

    public static string SaveKey => "SaveItemInventory";

    public event Action<ItemType, int> InventoryChanged;

    public IReadOnlyDictionary<ItemType, Inventory> Inventories => _inventories;

    [SerializeField]
    private SerializedDictionary<ItemType, Inventory> _inventories;
    private readonly Dictionary<Item, int> _itemIndexes = new();

    private void Awake()
    {
        foreach (var element in _inventories)
        {
            element.Value.Items = Enumerable.Repeat<Item>(null, element.Value.Capacity).ToList();
        }

        LoadSaveData();
    }

    public int AddItem(ItemData itemData, int count = 1)
    {
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
                int sameItemIndex = inventory.Items.FindIndex(item =>
                {
                    if (item == null)
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

                if (sameItemIndex != -1)
                {
                    var otherItem = inventory.Items[sameItemIndex] as CountableItem;
                    int prevCount = count;
                    count = otherItem.AddCountAndGetExcess(count);
                    Managers.Quest.ReceiveReport(Category.Item, itemData.ItemID, prevCount - count);
                    InventoryChanged?.Invoke(itemData.ItemType, sameItemIndex);
                }
                else
                {
                    if (TryGetEmptyIndex(itemData.ItemType, out var emptyIndex))
                    {
                        SetItem(countableItemData, emptyIndex, count);
                        count = Mathf.Max(count - countableItemData.MaxCount, 0);
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

    public void RemoveItem(Item item)
    {
        if (item == null)
        {
            return;
        }

        if (item.IsDestroyed)
        {
            return;
        }

        int index = _itemIndexes[item];
        DestroyItem(item.Data.ItemType, _itemIndexes[item]);
        InventoryChanged?.Invoke(item.Data.ItemType, index);
    }

    public void RemoveItem(ItemType itemType, int index)
    {
        if (IsEmptySlot(itemType, index))
        {
            return;
        }

        DestroyItem(itemType, index);
        InventoryChanged?.Invoke(itemType, index);
    }

    public void RemoveItem(string id, int count)
    {
        var itemType = GetItemTypeByID(id);

        for (int index = 0; index < _inventories[itemType].Items.Count; index++)
        {
            var item = _inventories[itemType].Items[index];

            if (item == null)
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
        if (fromIndex == toIndex)
        {
            return;
        }

        if (IsEmptySlot(itemType, fromIndex) || !IsEmptySlot(itemType, toIndex))
        {
            return;
        }

        var inventory = _inventories[itemType];
        if (inventory.Items[fromIndex] is not CountableItem countableItem)
        {
            return;
        }

        int remainingCount = countableItem.CurrentCount - count;
        if (remainingCount < 0)
        {
            return;
        }
        else if (remainingCount == 0)
        {
            SwapItem(itemType, fromIndex, toIndex);
        }
        else
        {
            countableItem.SetCount(remainingCount);
            inventory.Items[toIndex] = countableItem.CountableData.CreateItem(count);
            inventory.Count++;
            _itemIndexes.Add(inventory.Items[toIndex], toIndex);
        }

        InventoryChanged?.Invoke(itemType, fromIndex);
        InventoryChanged?.Invoke(itemType, toIndex);
    }

    public void SetItem(ItemData itemData, int index, int count = 1)
    {
        if (itemData == null)
        {
            Debug.Log("[ItemInventory/SetItem] itemData is null");
            return;
        }

        var inventory = _inventories[itemData.ItemType];

        if (IsEmptySlot(itemData.ItemType, index))
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

        _itemIndexes.Add(inventory.Items[index], index);
        Managers.Quest.ReceiveReport(Category.Item, itemData.ItemID, count);
        InventoryChanged?.Invoke(itemData.ItemType, index);
    }

    public T GetItem<T>(ItemType itemType, int index) where T : Item
    {
        return _inventories[itemType].Items[index] as T;
    }

    public int GetSameItemCount(string id)
    {
        var itemType = GetItemTypeByID(id);
        int count = 0;

        foreach (var item in _inventories[itemType].Items)
        {
            if (item == null)
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

    public int GetItemIndex(Item item)
    {
        if (_itemIndexes.TryGetValue(item, out var index))
        {
            return index;
        }

        return -1;
    }

    public bool IsEmptySlot(ItemType itemType, int index)
    {
        return _inventories[itemType].Items[index] == null;
    }

    public JArray CreateSaveData()
    {
        var saveData = new JArray();

        foreach (var element in _inventories)
        {
            for (int i = 0; i < element.Value.Capacity; i++)
            {
                var item = element.Value.Items[i];
                if (item == null)
                {
                    continue;
                }

                var itemSaveData = new ItemSaveData()
                {
                    ItemID = item.Data.ItemID,
                    Count = 1,
                    Index = i,
                };

                if (item is CountableItem countableItem)
                {
                    itemSaveData.Count = countableItem.CurrentCount;
                }

                saveData.Add(JObject.FromObject(itemSaveData));
            }
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
            var itemData = ItemDatabase.GetInstance.FindItemBy(itemSaveData.ItemID);
            if (itemData is CountableItemData countableItemData)
            {
                SetItem(countableItemData, itemSaveData.Index, itemSaveData.Count);
            }
            else
            {
                SetItem(itemData, itemSaveData.Index);
            }
        }
    }

    private void SwapItem(ItemType itemType, int Aindex, int BIndex)
    {
        var inventory = _inventories[itemType];

        if (!IsEmptySlot(itemType, Aindex))
        {
            _itemIndexes[inventory.Items[Aindex]] = BIndex;
        }

        if (!IsEmptySlot(itemType, BIndex))
        {
            _itemIndexes[inventory.Items[BIndex]] = Aindex;
        }

        (inventory.Items[Aindex], inventory.Items[BIndex]) = (inventory.Items[BIndex], inventory.Items[Aindex]);
    }

    private bool AddItemCountFromTo(ItemType itemType, int fromIndex, int toIndex)
    {
        if (IsEmptySlot(itemType, fromIndex) || IsEmptySlot(itemType, toIndex))
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

        int excessCount = toItem.AddCountAndGetExcess(fromItem.CurrentCount);
        fromItem.SetCount(excessCount);
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
        _itemIndexes.Remove(item);
        Managers.Quest.ReceiveReport(Category.Item, item.Data.ItemID, -count);
    }

    private bool TryGetEmptyIndex(ItemType itemType, out int index)
    {
        index = _inventories[itemType].Items.FindIndex(item => item == null);
        return index != -1;
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
        InventoryChanged = null;
    }
}
