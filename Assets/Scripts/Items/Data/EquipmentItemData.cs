using UnityEngine;

[CreateAssetMenu(menuName = "Item/Equipment", fileName = "Item_Equipment_")]
public class EquipmentItemData : ItemData
{
    [field: SerializeField]
    public EquipmentType EquipmentType { get; private set; }

    [field: SerializeField]
    public GameObject ItemPrefab { get; private set; }

    [field: SerializeField]
    public int HP { get; private set; }

    [field: SerializeField]
    public int MP { get; private set; }

    [field: SerializeField]
    public int SP { get; private set; }

    [field: SerializeField]
    public int Damage { get; private set; }

    [field: SerializeField]
    public int Defense { get; private set; }

    public EquipmentItemData()
    {
        ItemType = ItemType.Equipment;
    }

    public override Item CreateItem()
    {
        return new EquipmentItem(this);
    }
}
