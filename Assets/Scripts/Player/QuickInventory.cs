using System;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;

public class QuickInventory : MonoBehaviour
{
    public static readonly string SaveKey = "SaveQuickInventory";

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

    private void Start()
    {
        LoadSaveData();
    }

    public void SetUsable(IUsable usable, int index)
    {
        if (usable is null)
        {
            return;
        }

        _intUsable[index] = usable;
        InventoryChanged?.Invoke(index);
    }

    public void RemoveUsable(int index)
    {
        if (IsNullSlot(index))
        {
            return;
        }

        _intUsable[index] = null;
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

        if (usableA is null)
        {
            RemoveUsable(indexB);
        }

        if (usableB is null)
        {
            RemoveUsable(indexA);
        }

        SetUsable(usableA, indexB);
        SetUsable(usableB, indexA);
    }

    public bool IsNullSlot(int index)
    {
        return _intUsable[index] is null;
    }

    public JArray GetSaveData()
    {
        var saveDatas = new JArray();

        foreach (var element in _intUsable)
        {
            QuickSaveData saveData = new()
            {
                ItemSaveData = null,
                SkillSaveData = null,
            };

            if (element.Value is Item item)
            {
                ItemSaveData itemSaveData = new()
                {
                    ItemID = item.Data.ItemID,
                    Count = 1,
                    Index = item.Index,
                };

                if (item is CountableItem countableItem)
                {
                    itemSaveData.Count = countableItem.CurrentCount;
                }

                saveData.ItemSaveData = itemSaveData;
            }
            else if (element.Value is Skill skill)
            {
                SkillSaveData skillSaveData = new()
                {
                    SkillID = skill.Data.SkillID,
                    CurrentLevel = -1,
                };

                saveData.SkillSaveData = skillSaveData;
            }

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

        int i = 0;
        foreach (var data in datas)
        {
            var saveData = data.ToObject<QuickSaveData>();
            if (saveData.ItemSaveData.HasValue)
            {
                var itemSaveData = saveData.ItemSaveData.Value;
                var itemData = ItemDatabase.GetInstance.FindItemBy(itemSaveData.ItemID);
                var item = Player.ItemInventory.GetItem<Item>(itemData.ItemType, itemSaveData.Index);
                SetUsable(item as IUsable, i);
            }
            else if (saveData.SkillSaveData.HasValue)
            {
                var skillSaveData = saveData.SkillSaveData.Value;
                var skillData = SkillDatabase.GetInstance.FindSkillBy(skillSaveData.SkillID);
                var skill = Player.SkillTree.GetSkill(skillData);
                SetUsable(skill, i);
            }

            i++;
        }
    }
}
