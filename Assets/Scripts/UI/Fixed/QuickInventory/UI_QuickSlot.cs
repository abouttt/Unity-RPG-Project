using UnityEngine;
using UnityEngine.EventSystems;

public class UI_QuickSlot : UI_BaseSlot, IDropHandler
{
    enum Texts
    {
        CountText,
        KeyInfoText,
    }

    enum CooldownImages
    {
        CooldownImage,
    }

    public int Index { get; private set; }

    protected override void Init()
    {
        base.Init();

        BindText(typeof(Texts));
        Bind<UI_CooldownImage>(typeof(CooldownImages));

        Get<UI_CooldownImage>((int)CooldownImages.CooldownImage).gameObject.SetActive(false);
    }

    private void Start()
    {
        RefreshSlot();
    }

    public void InitSlot(int bindingIndex)
    {
        Index = bindingIndex;
        GetText((int)Texts.KeyInfoText).text = Managers.Input.GetBindingPath("Quick", bindingIndex);
    }

    public void RefreshSlot()
    {
        var usable = Player.QuickInventory.GetUsable(Index);
        if (usable is null)
        {
            Clear();
        }
        else if (ObjectRef != usable)
        {
            SetUsable(usable);
        }

        RefreshCountText();
    }

    private void SetUsable(IUsable usable)
    {
        if (usable != ObjectRef)
        {
            if (HasObject)
            {
                Clear();
            }

            if (usable is Item item)
            {
                SetObject(usable, item.Data.ItemImage);

                if (item is CountableItem countableItem)
                {
                    item.ItemChanged += RefreshCountText;
                    item.ItemChanged += CheckDestroyed;
                }

                if (item.Data is ICooldownable cooldownable)
                {
                    cooldownable.Cooldown.Cooldowned += ActiveCooldownImage;
                    Get<UI_CooldownImage>((int)CooldownImages.CooldownImage).SetCooldown(cooldownable.Cooldown);
                }
            }
        }
    }

    private new void Clear()
    {
        if (ObjectRef is Item item)
        {
            item.ItemChanged -= RefreshCountText;
            item.ItemChanged -= CheckDestroyed;

            if (item.Data is ICooldownable cooldownable)
            {
                cooldownable.Cooldown.Cooldowned -= ActiveCooldownImage;
            }
        }

        base.Clear();
        GetText((int)Texts.CountText).enabled = false;
        Get<UI_CooldownImage>((int)CooldownImages.CooldownImage).Clear();
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

    private void CheckDestroyed()
    {
        if (ObjectRef is Item item && item.IsDestroyed)
        {
            Player.QuickInventory.RemoveUsable(Index);
        }
    }

    private void ActiveCooldownImage()
    {
        Get<UI_CooldownImage>((int)CooldownImages.CooldownImage).gameObject.SetActive(true);
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        if (!CanPointerUp())
        {
            return;
        }

        (ObjectRef as IUsable).Use();
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {

    }

    public override void OnPointerExit(PointerEventData eventData)
    {

    }

    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag == gameObject)
        {
            return;
        }

        if (eventData.pointerDrag.TryGetComponent<UI_BaseSlot>(out var otherSlot))
        {
            if (!otherSlot.HasObject)
            {
                return;
            }

            switch (otherSlot.SlotType)
            {
                case SlotType.Item:
                    OnDropItemSlot(otherSlot as UI_ItemSlot);
                    break;
                case SlotType.Skill:
                    //OnDropSkillSlot(otherSlot as UI_SkillSlot);
                    break;
                case SlotType.Quick:
                    OnDropQuickSlot(otherSlot as UI_QuickSlot);
                    break;
            }
        }
    }

    private void OnDropItemSlot(UI_ItemSlot otherItemSlot)
    {
        if (otherItemSlot.ObjectRef is not IUsable usable)
        {
            return;
        }

        if (ObjectRef == otherItemSlot.ObjectRef)
        {
            return;
        }

        Player.QuickInventory.SetUsable(usable, Index);
    }

    //private void OnDropSkillSlot(UI_SkillSlot otherSkillSlot)
    //{
    //    if (otherSkillSlot.ObjectRef is not IUsable usable)
    //    {
    //        return;
    //    }

    //    if (ObjectRef == otherSkillSlot.ObjectRef)
    //    {
    //        return;
    //    }

    //    Player.QuickInventory.SetUsable(usable, Index);
    //}

    private void OnDropQuickSlot(UI_QuickSlot otherQuickSlot)
    {
        Player.QuickInventory.Swap(Index, otherQuickSlot.Index);
    }
}
