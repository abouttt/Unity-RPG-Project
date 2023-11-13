using UnityEngine;

public abstract class CountableItemData : ItemData
{
    [field: SerializeField]
    public int MaxCount { get; private set; } = 99;

    public abstract CountableItem CreateItem(int count);
}
