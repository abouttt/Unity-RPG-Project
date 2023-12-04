using UnityEngine;
using UnityEngine.UI;

public class UI_QuestTitleSubitem : UI_Base
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

    enum Toggles
    {
        QuestTrackerToggle,
    }

    public Quest QuestRef { get; private set; }

    protected override void Init()
    {
        BindButton(typeof(Buttons));
        BindText(typeof(Texts));
        Bind<Toggle>(typeof(Toggles));

        GetButton((int)Buttons.TitleButton).onClick.AddListener(() => Managers.UI.Get<UI_QuestPopup>().SetQuestDescription(QuestRef));
        GetText((int)Texts.CompleteText).gameObject.SetActive(false);
        Get<Toggle>((int)Toggles.QuestTrackerToggle).isOn = false;
        Get<Toggle>((int)Toggles.QuestTrackerToggle).onValueChanged.AddListener(toggle =>
        {
            if (toggle)
            {
                if (!Managers.UI.Get<UI_QuestTrackerFixed>().AddTracker(QuestRef))
                {
                    Get<Toggle>((int)Toggles.QuestTrackerToggle).isOn = false;
                }
            }
            else
            {
                Managers.UI.Get<UI_QuestTrackerFixed>().RemoveTracker(QuestRef);
            }
        });
    }

    public void SetQuestTitle(Quest quest)
    {
        QuestRef = quest;
        GetText((int)Texts.TitleText).text = $"[{quest.Data.LimitLevel}] {quest.Data.QuestName}";
    }

    public void ToggleCompleteText(bool toggle)
    {
        GetText((int)Texts.CompleteText).gameObject.SetActive(toggle);
    }

    public void ToggleQuestTracker(bool toggle)
    {
        if (Get<Toggle>((int)Toggles.QuestTrackerToggle).isOn != toggle)
        {
            Get<Toggle>((int)Toggles.QuestTrackerToggle).isOn = toggle;
        }
    }

    public bool IsShowedTracker()
    {
        return Get<Toggle>((int)Toggles.QuestTrackerToggle).isOn;
    }
}
