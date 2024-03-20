using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_SkillTreePopup : UI_Popup
{
    enum Texts
    {
        SkillPointAmountText,
    }

    enum Buttons
    {
        ResetButton,
        CloseButton,
    }

    protected override void Init()
    {
        base.Init();

        BindText(typeof(Texts));
        BindButton(typeof(Buttons));

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
