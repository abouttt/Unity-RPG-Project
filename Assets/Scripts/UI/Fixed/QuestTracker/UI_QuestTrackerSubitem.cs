using System.Text;
using UnityEngine;

public class UI_QuestTrackerSubitem : UI_Base
{
    enum Texts
    {
        QuestNameText,
        QuestTargetText,
    }

    enum Buttons
    {
        CloseButton,
    }

    [SerializeField]
    private Color _completableTextColor = Color.black;

    private Quest _questRef;
    private readonly StringBuilder _sb = new(50);
    private QuestState _questState;

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
        _questState = QuestState.Active;
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

        foreach (var element in _questRef.Targets)
        {
            int completeCount = element.Key.CompleteCount;
            int currentCount = Mathf.Clamp(element.Value, 0, completeCount);
            _sb.AppendLine($"- {element.Key.Description} ({currentCount}/{completeCount})");
        }

        GetText((int)Texts.QuestTargetText).text = _sb.ToString();

        if (_questState != _questRef.State)
        {
            _questState = _questRef.State;

            if (_questRef.State is QuestState.Completable)
            {
                GetText((int)Texts.QuestNameText).color = _completableTextColor;
                GetText((int)Texts.QuestTargetText).color = _completableTextColor;
            }
            else
            {
                GetText((int)Texts.QuestNameText).color = Color.white;
                GetText((int)Texts.QuestTargetText).color = Color.white;
            }
        }
    }
}
