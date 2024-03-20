using UnityEngine;

public class UI_NPCQuestTitleSubitem : UI_Base
{
    enum Texts
    {
        TitleText,
        CompleteText,
    }

    enum Buttons
    {
        TitleButton,
    }

    public QuestData QuestDataRef { get; private set; }

    protected override void Init()
    {
        BindText(typeof(Texts));
        BindButton(typeof(Buttons));

        GetText((int)Texts.CompleteText).gameObject.SetActive(false);
        GetButton((int)Buttons.TitleButton).onClick.AddListener(() => Managers.UI.Get<UI_NPCQuestPopup>().SetQuestDescription(QuestDataRef));
    }

    public void SetQuestTitle(QuestData questData)
    {
        QuestDataRef = questData;
        GetText((int)Texts.TitleText).text = $"[{questData.LimitLevel}] {questData.QuestName}";
        GetText((int)Texts.CompleteText).gameObject.SetActive(Managers.Quest.IsSameState(questData, QuestState.Completable));
    }

    public void ToggleCompleteText(bool toggle)
    {
        GetText((int)Texts.CompleteText).gameObject.SetActive(toggle);
    }
}
