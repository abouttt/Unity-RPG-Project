using UnityEngine;

public class UI_NPCQuestTitleSubitem : UI_Base
{
    enum Buttons
    {
        TitleButton,
    }

    enum Texts
    {
        TitleText,
        CompleteText,
    }

    public QuestData QuestDataRef { get; private set; }

    protected override void Init()
    {
        BindButton(typeof(Buttons));
        BindText(typeof(Texts));

        GetButton((int)Buttons.TitleButton).onClick.AddListener(() => Managers.UI.Get<UI_NPCQuestPopup>().SetQuestDescription(QuestDataRef));
        GetText((int)Texts.CompleteText).gameObject.SetActive(false);
    }

    public void SetQuestTitle(QuestData questData)
    {
        QuestDataRef = questData;
        GetText((int)Texts.TitleText).text = $"[{questData.LimitLevel}] {questData.QuestName}";
    }

    public void ToggleCompleteText(bool toggle)
    {
        GetText((int)Texts.CompleteText).gameObject.SetActive(toggle);
    }
}
