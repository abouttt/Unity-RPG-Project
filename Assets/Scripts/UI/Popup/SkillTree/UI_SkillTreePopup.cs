using UnityEngine;

public class UI_SkillTreePopup : UI_Popup
{
    enum Buttons
    {
        ResetButton,
        CloseButton,
    }

    enum Texts
    {
        SkillPointAmountText,
    }

    protected override void Init()
    {
        base.Init();

        BindButton(typeof(Buttons));
        BindText(typeof(Texts));

        GetButton((int)Buttons.ResetButton).onClick.AddListener(Player.SkillTree.ResetSkills);
        GetButton((int)Buttons.CloseButton).onClick.AddListener(Managers.UI.Close<UI_SkillTreePopup>);

        Player.Status.SkillPointChanged += RefreshSkillPointText;
    }

    private void Start()
    {
        Managers.UI.Register<UI_SkillTreePopup>(this);

        RefreshSkillPointText();
    }

    private void RefreshSkillPointText()
    {
        GetText((int)Texts.SkillPointAmountText).text = Player.Status.SkillPoint.ToString();
    }
}
