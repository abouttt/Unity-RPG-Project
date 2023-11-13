using UnityEngine;

public abstract class ItemData : ScriptableObject
{
    [field: SerializeField]
    public string ItemID { get; private set; }

    [field: SerializeField]
    public string ItemName { get; private set; }

    [field: SerializeField]
    public Sprite ItemImage { get; private set; }

    public ItemType ItemType { get; protected set; }

    [field: SerializeField]
    public ItemQuality ItemQuality { get; private set; }

    [field: SerializeField, TextArea]
    public string Description { get; private set; }

    [field: SerializeField]
    public int LimitLevel { get; private set; }

    [field: SerializeField]
    public int Price { get; private set; }

    public abstract Item CreateItem();

    public bool Equals(ItemData other)
    {
        if (other == null)
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        if (GetType() != other.GetType())
        {
            return false;
        }

        return ItemID == other.ItemID;
    }
}
