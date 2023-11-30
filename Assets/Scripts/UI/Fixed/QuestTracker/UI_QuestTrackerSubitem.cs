using System.Text;
using UnityEngine;

public class UI_QuestTrackerSubitem : UI_Base
{
    enum Buttons
    {
        CloseButton,
    }

    enum Texts
    {
        QuestNameText,
        QuestTargetText,
    }

    private Quest _questRef;
    private readonly StringBuilder _sb = new(50);

    protected override void Init()
    {
        BindButton(typeof(Buttons));
        BindText(typeof(Texts));

        GetButton((int)Buttons.CloseButton).onClick.AddListener(() => 
        { 
            Managers.UI.Get<UI_QuestPopup>().ToggleQuestTracker(_questRef, false);
        });
    }

    public void SetQuest(Quest quest)
    {
        _questRef = quest;
        quest.TargetCountChanged += RefreshTargetText;
        GetText((int)Texts.QuestNameText).text = $"<{quest.Data.QuestName}>";
        RefreshTargetText();
    }

    public void Clear()
    {
        _questRef.TargetCountChanged -= RefreshTargetText;
        _questRef = null;
    }

    private void RefreshTargetText()
    {
        _sb.Clear();

        foreach (var target in _questRef.Targets)
        {
            var completeCount = target.Key.CompleteCount;
            var currentCount = Mathf.Clamp(target.Value, 0, completeCount);
            _sb.AppendLine($"- {target.Key.Description} ({currentCount}/{completeCount})");
        }

        GetText((int)Texts.QuestTargetText).text = _sb.ToString();
    }
}
