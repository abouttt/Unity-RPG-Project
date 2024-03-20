using UnityEngine;
using UnityEngine.EventSystems;

public class UI_SkillSlot : UI_BaseSlot
{
    enum Imagess
    {
        SkillFrame = 2,
        SkillDisabledImage,
        LevelUpDisabledImage,
    }

    enum Texts
    {
        LevelText,
    }

    enum Buttons
    {
        LevelUpButton,
    }

    enum CooldownImages
    {
        CooldownImage,
    }

    [field: SerializeField]
    public SkillData SkillData { get; private set; }

    private Skill SkillRef => ObjectRef as Skill;

    protected override void Init()
    {
        base.Init();

        CanDrag = false;

        BindImage(typeof(Imagess));
        BindText(typeof(Texts));
        BindButton(typeof(Buttons));
        Bind<UI_CooldownImage>(typeof(CooldownImages));

        GetImage((int)Imagess.SkillFrame).sprite = SkillData.SkillFrame;
        GetImage((int)Imagess.LevelUpDisabledImage).gameObject.SetActive(false);
        GetButton((int)Buttons.LevelUpButton).gameObject.SetActive(false);
        GetButton((int)Buttons.LevelUpButton).onClick.AddListener(() =>
        {
            SkillRef.LevelUp();
        });

        SkillData.Cooldown.Cooldowned += ActiveCooldownImage;
        Get<UI_CooldownImage>((int)CooldownImages.CooldownImage).SetCooldown(SkillData.Cooldown);

        SetObject(Player.SkillTree.GetSkillBy(SkillData), SkillData.SkillImage);
        SkillRef.SkillChanged += Refresh;
    }

    private void Start()
    {
        RefreshLevelText();
    }

    private void Refresh()
    {
        RefreshLevelText();

        if (SkillRef.CurrentLevel == SkillData.MaxLevel)
        {
            GetImage((int)Imagess.LevelUpDisabledImage).gameObject.SetActive(false);
            GetButton((int)Buttons.LevelUpButton).gameObject.SetActive(false);
            return;
        }

        CanDrag = !SkillRef.IsLock && SkillData.SkillType is SkillType.Active;
        GetButton((int)Buttons.LevelUpButton).gameObject.SetActive(SkillRef.IsAcquirable);
        GetImage((int)Imagess.LevelUpDisabledImage).gameObject.SetActive(Player.Status.SkillPoint < SkillData.RequiredSkillPoint);

        if (!SkillRef.IsAcquirable && SkillRef.IsLock)
        {
            CanDrag = false;
            GetImage((int)Imagess.SkillDisabledImage).gameObject.SetActive(true);
            GetImage((int)Imagess.LevelUpDisabledImage).gameObject.SetActive(false);
            GetButton((int)Buttons.LevelUpButton).gameObject.SetActive(false);
        }
        else if (!SkillRef.IsLock)
        {
            GetImage((int)Imagess.SkillDisabledImage).gameObject.SetActive(false);
        }
    }

    private void RefreshLevelText()
    {
        GetText((int)Texts.LevelText).text = $"{SkillRef.CurrentLevel} / {SkillData.MaxLevel}";
    }

    private void ActiveCooldownImage()
    {
        Get<UI_CooldownImage>((int)CooldownImages.CooldownImage).gameObject.SetActive(true);
    }

    private bool IsOnPointerSameGameObject(PointerEventData eventData, GameObject gameObject)
    {
        return eventData.pointerCurrentRaycast.gameObject == gameObject;
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);
        Managers.UI.Get<UI_SkillTreePopup>().SetTop();
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        if (!CanPointerUp() || SkillRef.IsLock)
        {
            return;
        }

        if (IsOnPointerSameGameObject(eventData, GetButton((int)Buttons.LevelUpButton).gameObject) ||
            IsOnPointerSameGameObject(eventData, GetImage((int)Imagess.LevelUpDisabledImage).gameObject))
        {
            return;
        }

        SkillRef.Use();
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        if (IsOnPointerSameGameObject(eventData, GetButton((int)Buttons.LevelUpButton).gameObject) ||
            IsOnPointerSameGameObject(eventData, GetImage((int)Imagess.LevelUpDisabledImage).gameObject))
        {
            return;
        }

        Managers.UI.Get<UI_SkillTooltipTop>().Target = this;
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        Managers.UI.Get<UI_SkillTooltipTop>().Target = null;
    }
}
