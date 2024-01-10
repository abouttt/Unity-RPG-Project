using UnityEngine;
using UnityEngine.EventSystems;

public class UI_ItemSlot : UI_BaseSlot, IDropHandler
{
    enum Texts
    {
        CountText,
    }

    enum CooldownImages
    {
        CooldownImage,
    }

    public int Index { get; set; } = -1;

    protected override void Init()
    {
        base.Init();

        BindText(typeof(Texts));
        Bind<UI_CooldownImage>(typeof(CooldownImages));

        Get<UI_CooldownImage>((int)CooldownImages.CooldownImage).gameObject.SetActive(false);
    }

    public void SetItem(Item item)
    {
        if (ObjectRef != item)
        {
            if (HasObject)
            {
                Clear();
            }

            SetObject(item, item.Data.ItemImage);

            if (item is CountableItem)
            {
                item.ItemChanged += RefreshCountText;
            }

            if (item.Data is ICooldownable cooldownable)
            {
                cooldownable.Cooldown.Cooldowned += ActiveCooldownImage;
                Get<UI_CooldownImage>((int)CooldownImages.CooldownImage).SetCooldown(cooldownable.Cooldown);
            }

            RefreshCountText();
        }
    }

    public override void Clear()
    {
        if (ObjectRef is Item item)
        {
            if (item is CountableItem)
            {
                item.ItemChanged -= RefreshCountText;
            }

            if (item.Data is ICooldownable cooldownable)
            {
                cooldownable.Cooldown.Cooldowned -= ActiveCooldownImage;
                Get<UI_CooldownImage>((int)CooldownImages.CooldownImage).Clear();
            }
        }

        base.Clear();
        RefreshCountText();
    }

    private void RefreshCountText()
    {
        if (ObjectRef is CountableItem countableItem && countableItem.CurrentCount > 1)
        {
            GetText((int)Texts.CountText).enabled = true;
            GetText((int)Texts.CountText).text = countableItem.CurrentCount.ToString();
        }
        else
        {
            GetText((int)Texts.CountText).enabled = false;
        }
    }

    private void ActiveCooldownImage()
    {
        Get<UI_CooldownImage>((int)CooldownImages.CooldownImage).gameObject.SetActive(true);
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);
        Managers.UI.Get<UI_ItemInventoryPopup>().SetTop();
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        if (!CanPointerUp())
        {
            return;
        }

        if (Managers.UI.IsOn<UI_ShopPopup>())
        {
            Managers.UI.Get<UI_ShopPopup>().SellItem(this);
        }
        else
        {
            var item = ObjectRef as Item;
            switch (item.Data.ItemType)
            {
                case ItemType.Equipment:
                    if (ObjectRef is EquipmentItem equipmentItem)
                    {
                        if (Player.EquipmentInventory.IsNullSlot(equipmentItem.EquipmentData.EquipmentType))
                        {
                            Player.ItemInventory.RemoveItem(equipmentItem.Data.ItemType, Index);
                        }
                        else
                        {
                            var otherEquipmentItem = Player.EquipmentInventory.GetItem(equipmentItem.EquipmentData.EquipmentType);
                            Player.ItemInventory.SetItem(otherEquipmentItem.Data, Index);
                        }

                        Player.EquipmentInventory.EquipItem(equipmentItem.EquipmentData);
                    }
                    break;
                default:
                    if (item is IUsable usable)
                    {
                        usable.Use();
                    }
                    break;
            }
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag == gameObject)
        {
            return;
        }

        if (eventData.pointerDrag.TryGetComponent<UI_BaseSlot>(out var otherSlot))
        {
            switch (otherSlot.SlotType)
            {
                case SlotType.Item:
                    OnDropItemSlot(otherSlot as UI_ItemSlot);
                    break;
                case SlotType.Equipment:
                    OnDropEquipmentSlot(otherSlot as UI_EquipmentSlot);
                    break;
                default:
                    break;
            }
        }
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        Managers.UI.Get<UI_ItemTooltipTop>().Target = this;
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        Managers.UI.Get<UI_ItemTooltipTop>().Target = null;
    }

    private void OnDropItemSlot(UI_ItemSlot otherItemSlot)
    {
        var otherItem = otherItemSlot.ObjectRef as Item;

        if (!HasObject && otherItem is CountableItem otherCountableItem && otherCountableItem.CurrentCount > 1)
        {
            var splitPopup = Managers.UI.Show<UI_ItemSplitPopup>();
            splitPopup.SetEvent(() =>
                Player.ItemInventory.SplitItem(otherItem.Data.ItemType, otherItemSlot.Index, Index, splitPopup.CurrentCount),
                $"[{otherCountableItem.Data.ItemName}] 아이템 나누기", 1, otherCountableItem.CurrentCount);
        }
        else
        {
            Player.ItemInventory.MoveItem(otherItem.Data.ItemType, otherItemSlot.Index, Index);
        }
    }

    private void OnDropEquipmentSlot(UI_EquipmentSlot otherEquipmentSlot)
    {
        var otherEquipmentData = (otherEquipmentSlot.ObjectRef as EquipmentItem).EquipmentData;

        if (HasObject)
        {
            var thisEquipmentData = (ObjectRef as EquipmentItem).EquipmentData;

            if (thisEquipmentData.EquipmentType != otherEquipmentData.EquipmentType)
            {
                return;
            }

            if (thisEquipmentData.LimitLevel > Player.Status.Level)
            {
                return;
            }

            Player.EquipmentInventory.EquipItem(thisEquipmentData);
        }
        else
        {
            Player.EquipmentInventory.UnequipItem(otherEquipmentSlot.EquipmentType);
        }

        Player.ItemInventory.SetItem(otherEquipmentData, Index);
    }
}
