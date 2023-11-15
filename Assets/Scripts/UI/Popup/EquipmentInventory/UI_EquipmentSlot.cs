using UnityEngine;
using UnityEngine.EventSystems;

public class UI_EquipmentSlot : UI_BaseSlot, IDropHandler
{
    [field: SerializeField]
    public EquipmentType EquipmentType { get; private set; }

    public void SetItem(EquipmentItem item)
    {
        SetObject(item, item.Data.ItemImage);
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);
        Managers.UI.Get<UI_EquipmentInventoryPopup>().SetTop();
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        if (!CanPointerUp())
        {
            return;
        }

        Player.ItemInventory.AddItem((ObjectRef as Item).Data);
        Player.EquipmentInventory.UnequipItem(EquipmentType);
    }

    public override void OnBeginDrag(PointerEventData eventData)
    {
        base.OnBeginDrag(eventData);

        if (eventData.pointerDrag == null)
        {
            return;
        }

        if (Managers.UI.IsOn<UI_ItemInventoryPopup>())
        {
            Managers.UI.Get<UI_ItemInventoryPopup>().OpenTheTab(ItemType.Equipment);
        }
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        //Managers.UI.Get<UI_ItemTooltipTop>().Target = this;
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        //Managers.UI.Get<UI_ItemTooltipTop>().Target = null;
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag == gameObject)
        {
            return;
        }

        if (eventData.pointerDrag.TryGetComponent<UI_ItemSlot>(out var otherItemSlot))
        {
            if (otherItemSlot.ObjectRef is not EquipmentItem otherItem)
            {
                return;
            }

            if (EquipmentType != otherItem.EquipmentData.EquipmentType)
            {
                return;
            }

            //if (otherItem.EquipmentData.LimitLevel > Player.Status.Level)
            //{
            //    return;
            //}

            if (HasObject)
            {
                Player.ItemInventory.SetItem((ObjectRef as EquipmentItem).EquipmentData, otherItemSlot.Index);
            }
            else
            {
                Player.ItemInventory.RemoveItem(ItemType.Equipment, otherItemSlot.Index);
            }

            Player.EquipmentInventory.EquipItem(otherItem.EquipmentData);
        }
    }
}
