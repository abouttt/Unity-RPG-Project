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
                    item.ItemChanged += CheckItemDestroyed;
                }

                if (item.Data is ICooldownable cooldownable)
                {
                    cooldownable.Cooldown.Cooldowned += ActiveCooldownImage;
                    Get<UI_CooldownImage>((int)CooldownImages.CooldownImage).SetCooldown(cooldownable.Cooldown);
                }
            }
            else if (usable is Skill skill)
            {
                SetObject(usable, skill.Data.SkillImage);

                skill.SkillChanged += CheckSkillLocked;

                if (skill.Data is ICooldownable cooldownable)
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
            item.ItemChanged -= CheckItemDestroyed;

            if (item.Data is ICooldownable cooldownable)
            {
                cooldownable.Cooldown.Cooldowned -= ActiveCooldownImage;
            }
        }
        else if (ObjectRef is Skill skill)
        {
            skill.SkillChanged -= CheckSkillLocked;

            if (skill.Data is ICooldownable cooldownable)
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

    private void CheckItemDestroyed()
    {
        if (ObjectRef is Item item && item.IsDestroyed)
        {
            Player.QuickInventory.RemoveUsable(Index);
        }
    }

    private void CheckSkillLocked()
    {
        if (ObjectRef is Skill skill && skill.IsLock)
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
        Managers.UI.Get<UI_ItemTooltipTop>().Target = this;
        Managers.UI.Get<UI_SkillTooltipTop>().Target = this;
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        Managers.UI.Get<UI_ItemTooltipTop>().Target = null;
        Managers.UI.Get<UI_SkillTooltipTop>().Target = null;
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
                case SlotType.Skill:
                    if (otherSlot.ObjectRef is not IUsable usable)
                    {
                        return;
                    }
                    if (ObjectRef == usable)
                    {
                        return;
                    }
                    Player.QuickInventory.SetUsable(usable, Index);
                    break;

                case SlotType.Quick:
                    Player.QuickInventory.Swap(Index, (otherSlot as UI_QuickSlot).Index);
                    break;
            }
        }
    }
}
