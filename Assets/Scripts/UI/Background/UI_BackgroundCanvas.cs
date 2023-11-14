using UnityEngine;
using UnityEngine.EventSystems;

public class UI_BackgroundCanvas : UI_Base, IPointerClickHandler, IDropHandler
{
    [SerializeField, Space(10), TextArea]
    private string DestroyItemText;

    protected override void Init()
    {
        Managers.UI.Register<UI_BackgroundCanvas>(this);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Managers.Input.ToggleCursor(false);
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag.TryGetComponent<UI_BaseSlot>(out var otherSlot))
        {
            switch (otherSlot.SlotType)
            {
                case SlotType.Item:
                    OnDropItemSlot(otherSlot as UI_ItemSlot);
                    break;
                case SlotType.Equipment:
                    //OnDropEquipmentSlot(otherSlot as UI_EquipmentSlot);
                    break;
                case SlotType.Quick:
                    //OnDropQuickSlot(otherSlot as UI_QuickSlot);
                    break;
                default:
                    break;
            }
        }
    }

    private void OnDropItemSlot(UI_ItemSlot itemSlot)
    {
        var itemDestructionConfirmation = Managers.UI.Show<UI_ConfirmationPopup>();
        var item = itemSlot.ObjectRef as Item;
        var text = $"[{item.Data.ItemName}] {DestroyItemText}";
        itemDestructionConfirmation.SetEvent(() =>
        {
            Player.ItemInventory.RemoveItem(item.Data.ItemType, itemSlot.Index);
        },
        text);
    }

    //private void OnDropEquipmentSlot(UI_EquipmentSlot equipmentSlot)
    //{
    //    var equipmentItem = equipmentSlot.ObjectRef as EquipmentItem;
    //    Player.ItemInventory.AddItem(equipmentItem.Data);
    //    Player.EquipmentInventory.UnequipItem(equipmentItem.EquipmentData.EquipmentType);
    //}
    //
    //private void OnDropQuickSlot(UI_QuickSlot quickSlot)
    //{
    //    Player.QuickInventory.Clear(quickSlot.Index);
    //}
}
