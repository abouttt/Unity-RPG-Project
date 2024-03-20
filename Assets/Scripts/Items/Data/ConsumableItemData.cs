using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Item/Consumable", fileName = "Item_Consumable_")]
public class ConsumableItemData : CountableItemData, ICooldownable
{
    [field: SerializeField]
    public int RequiredCount { get; private set; } = 1;

    [field: SerializeField]
    public string ItemClassName { get; private set; }

    [field: SerializeField]
    public Cooldown Cooldown { get; set; }

    public ConsumableItemData()
    {
        ItemType = ItemType.Consumable;
    }

    public override Item CreateItem()
    {
        return GetInstance();
    }

    public override CountableItem CreateItem(int count)
    {
        return GetInstance(count);
    }

    private ConsumableItem GetInstance(int count = 1)
    {
        var type = Type.GetType(ItemClassName);
        if (type != null)
        {
            return Activator.CreateInstance(type, this, count) as ConsumableItem;
        }

        return null;
    }
}
