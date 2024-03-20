using System;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;

public class QuickInventory : MonoBehaviour, ISavable
{
    public static string SaveKey => "SaveQuickInventory";

    public event Action<int> InventoryChanged;

    [field: SerializeField]
    public int Capacity { get; private set; }

    private readonly Dictionary<int, IUsable> _usables = new();

    private void Awake()
    {
        for (int i = 0; i < Capacity; i++)
        {
            _usables.Add(i, null);
        }

        LoadSaveData();
    }

    public void SetUsable(IUsable usable, int index)
    {
        if (usable == null)
        {
            return;
        }

        _usables[index] = usable;
        InventoryChanged?.Invoke(index);
    }

    public void RemoveUsable(int index)
    {
        if (IsEmptySlot(index))
        {
            return;
        }

        _usables[index] = null;
        InventoryChanged?.Invoke(index);
    }

    public IUsable GetUsable(int index)
    {
        return _usables[index];
    }

    public void Swap(int indexA, int indexB)
    {
        var usableA = _usables[indexA];
        var usableB = _usables[indexB];

        if (usableA == null)
        {
            RemoveUsable(indexB);
        }

        if (usableB == null)
        {
            RemoveUsable(indexA);
        }

        SetUsable(usableA, indexB);
        SetUsable(usableB, indexA);
    }

    public bool IsEmptySlot(int index)
    {
        return _usables[index] == null;
    }

    public JArray CreateSaveData()
    {
        var saveData = new JArray();

        foreach (var element in _usables)
        {
            var quickSaveData = new QuickSaveData()
            {
                ItemSaveData = null,
                SkillSaveData = null,
            };

            if (element.Value is Item item)
            {
                var itemSaveData = new ItemSaveData()
                {
                    ItemID = item.Data.ItemID,
                    Count = 1,
                    Index = Player.ItemInventory.GetItemIndex(item),
                };

                if (item is CountableItem countableItem)
                {
                    itemSaveData.Count = countableItem.CurrentCount;
                }

                quickSaveData.ItemSaveData = itemSaveData;
            }
            else if (element.Value is Skill skill)
            {
                var skillSaveData = new SkillSaveData()
                {
                    SkillID = skill.Data.SkillID,
                    CurrentLevel = -1,
                };

                quickSaveData.SkillSaveData = skillSaveData;
            }

            saveData.Add(JObject.FromObject(quickSaveData));
        }

        return saveData;
    }

    public void LoadSaveData()
    {
        if (!Managers.Data.Load<JArray>(SaveKey, out var saveData))
        {
            return;
        }

        int i = 0;
        foreach (var token in saveData)
        {
            var quickSaveData = token.ToObject<QuickSaveData>();
            if (quickSaveData.ItemSaveData.HasValue)
            {
                var itemSaveData = quickSaveData.ItemSaveData.Value;
                var itemData = ItemDatabase.GetInstance.FindItemBy(itemSaveData.ItemID);
                var item = Player.ItemInventory.GetItem<Item>(itemData.ItemType, itemSaveData.Index);
                SetUsable(item as IUsable, i);
            }
            else if (quickSaveData.SkillSaveData.HasValue)
            {
                var skillSaveData = quickSaveData.SkillSaveData.Value;
                var skillData = SkillDatabase.GetInstance.FindSkillBy(skillSaveData.SkillID);
                var skill = Player.SkillTree.GetSkillBy(skillData);
                SetUsable(skill, i);
            }

            i++;
        }
    }

    private void OnDestroy()
    {
        InventoryChanged = null;
    }
}
