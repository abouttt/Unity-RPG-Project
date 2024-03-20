using UnityEngine;

public class HealPotion : ConsumableItem
{
    public HealPotion(ConsumableItemData data, int count = 1)
        : base(data, count)
    { }

    public override bool Use()
    {
        if (!CheckCanUseAndSubCount())
        {
            return false;
        }

        Player.Status.HP += 100;
        Managers.Resource.Instantiate("HealOnceBurst.prefab", Player.Collider.bounds.center, null, true);

        return true;
    }
}
