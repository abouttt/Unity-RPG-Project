using UnityEngine;

public abstract class ConsumableItem : CountableItem, IUsable
{
    public ConsumableItemData ConsumableData { get; private set; }

    public ConsumableItem(ConsumableItemData data, int count = 1)
        : base(data, count)
    {
        ConsumableData = data;
    }

    public abstract bool Use();

    protected bool CheckCanUseAndSubCount()
    {
        if (Player.Status.HP <= 0)
        {
            return false;
        }

        if (Player.Status.Level < Data.LimitLevel)
        {
            return false;
        }

        if (ConsumableData.Cooldown.Current > 0f)
        {
            return false;
        }

        int remainingCount = CurrentCount - ConsumableData.RequiredCount;
        if (remainingCount < 0)
        {
            return false;
        }

        SetCount(remainingCount);

        if (IsEmpty)
        {
            Player.ItemInventory.RemoveItem(this);
        }

        ConsumableData.Cooldown.OnCooldowned();
        Managers.Quest.ReceiveReport(Category.Item, Data.ItemID, -ConsumableData.RequiredCount);

        return true;
    }
}
