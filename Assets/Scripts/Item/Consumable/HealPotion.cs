using UnityEngine;

public class HealPotion : ConsumableItem
{
    public HealPotion(ConsumableItemData data, int count)
        : base(data, count)
    { }

    public override bool Use()
    {
        if (!CheckCanUseAndSubtractCount())
        {
            return false;
        }

        //Player.Status.HP += 100;

        return true;
    }
}
