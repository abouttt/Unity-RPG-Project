using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Newtonsoft.Json.Linq;

public class UI_QuestPopup : UI_Popup
{
    public static readonly string SaveKey = "SaveQuestUI";

    enum GameObjects
    {
        QuestInfo,
    }

    enum RectTransforms
    {
        QuestTitleSubitems,
        QuestRewardSlots,
    }

    enum Buttons
    {
        CloseButton,
        CompleteButton,
        CancelButton,
    }

    enum Texts
    {
        QuestTitleText,
        QuestDescriptionText,
        QuestTargetText,
        QuestRewardText,
        NOQuestText,
    }

    private Quest _selectedQuest;
    private readonly Dictionary<Quest, UI_QuestTitleSubitem> _titleSubitems = new();
    private readonly StringBuilder _sb = new(50);

    protected override void Init()
    {
        base.Init();

        BindObject(typeof(GameObjects));
        BindRT(typeof(RectTransforms));
        BindButton(typeof(Buttons));
        BindText(typeof(Texts));

        GetButton((int)Buttons.CloseButton).onClick.AddListener(Managers.UI.Close<UI_QuestPopup>);
        GetButton((int)Buttons.CompleteButton).onClick.AddListener(() => Managers.Quest.Complete(_selectedQuest));
        GetButton((int)Buttons.CancelButton).onClick.AddListener(() => Managers.Quest.Unregister(_selectedQuest));

        Managers.Quest.QuestRegistered += OnQuestRegisterd;
        Managers.Quest.QuestCompletabled += OnQuestCompletabled;
        Managers.Quest.QuestCompletableCanceled += OnQuestCompletableCanceld;
        Managers.Quest.QuestCompleted += OnQuestCompletedOrCanceled;
        Managers.Quest.QuestUnRegistered += OnQuestCompletedOrCanceled;

        Managers.Game.GameStarted += LoadSaveData;

        Clear();
    }

    private void Start()
    {
        Managers.UI.Register<UI_QuestPopup>(this);
    }

    public void SetQuestDescription(Quest quest)
    {
        if (quest is null)
        {
            return;
        }

        if (_selectedQuest == quest)
        {
            return;
        }

        Clear();

        _selectedQuest = quest;
        quest.TargetCountChanged += RefreshTargetText;
        GetObject((int)GameObjects.QuestInfo).SetActive(true);
        GetText((int)Texts.QuestTitleText).text = quest.Data.QuestName;
        GetText((int)Texts.QuestDescriptionText).text = quest.Data.Description;
        RefreshTargetText();
        SetRewardText(quest.Data);
        ToggleCompleteButton(quest, quest.State is QuestState.Completable);
        GetButton((int)Buttons.CancelButton).gameObject.SetActive(true);
        GetText((int)Texts.NOQuestText).gameObject.SetActive(false);
    }

    public void ToggleQuestTracker(Quest quest, bool toggle)
    {
        if (_titleSubitems.TryGetValue(quest, out var subitem))
        {
            subitem.ToggleQuestTracker(toggle);
        }
    }

    public JArray GetSaveData()
    {
        var saveDatas = new JArray();

        foreach (var element in _titleSubitems)
        {
            if (!element.Value.IsShowedTracker())
            {
                continue;
            }

            QuestUISaveData saveData = new()
            {
                QuestID = element.Key.Data.QuestID
            };

            saveDatas.Add(JObject.FromObject(saveData));
        }

        return saveDatas;
    }

    private void OnQuestRegisterd(Quest quest)
    {
        var go = Managers.Resource.Instantiate("UI_QuestTitleSubitem", GetRT((int)RectTransforms.QuestTitleSubitems), true);
        var questTitleSubitem = go.GetComponent<UI_QuestTitleSubitem>();
        questTitleSubitem.SetQuestTitle(quest);
        _titleSubitems.Add(quest, questTitleSubitem);
    }

    private void OnQuestCompletabled(Quest quest)
    {
        if (_titleSubitems.TryGetValue(quest, out var subitem))
        {
            subitem.ToggleCompleteText(true);
            ToggleCompleteButton(quest, _selectedQuest == quest);
        }
    }

    private void OnQuestCompletableCanceld(Quest quest)
    {
        if (_titleSubitems.TryGetValue(quest, out var subitem))
        {
            subitem.ToggleCompleteText(false);
            ToggleCompleteButton(quest, !(_selectedQuest == quest));
        }
    }

    private void OnQuestCompletedOrCanceled(Quest quest)
    {
        if (_titleSubitems.TryGetValue(quest, out var subitem))
        {
            subitem.ToggleCompleteText(false);
            subitem.ToggleQuestTracker(false);
            Managers.Resource.Destroy(subitem.gameObject);
            _titleSubitems.Remove(quest);
            Clear();
        }
    }

    private void RefreshTargetText()
    {
        _sb.Clear();
        _sb.AppendLine("[목적]");

        foreach (var target in _selectedQuest.Targets)
        {
            var completeCount = target.Key.CompleteCount;
            var currentCount = Mathf.Clamp(target.Value, 0, completeCount);
            _sb.AppendLine($"{target.Key.Description} ({currentCount}/{completeCount})");
        }

        GetText((int)Texts.QuestTargetText).text = _sb.ToString();
    }

    private void SetRewardText(QuestData questData)
    {
        _sb.Clear();
        _sb.AppendLine("[보상]");

        if (questData.RewardGold > 0)
        {
            _sb.AppendLine($"{questData.RewardGold} Gold");
        }

        if (questData.RewardXP > 0)
        {
            _sb.AppendLine($"{questData.RewardXP} XP");
        }

        GetText((int)Texts.QuestRewardText).text = _sb.ToString();

        foreach (var element in questData.RewardItems)
        {
            var go = Managers.Resource.Instantiate("UI_QuestRewardSlot", GetRT((int)RectTransforms.QuestRewardSlots), true);
            go.GetComponent<UI_QuestRewardSlot>().SetReward(element.Key, element.Value);
        }
    }

    private void ToggleCompleteButton(Quest quest, bool toggle)
    {
        if (quest.Data.CanRemoteComplete)
        {
            GetButton((int)Buttons.CompleteButton).gameObject.SetActive(toggle);
        }
    }

    private void Clear()
    {
        if (_selectedQuest is not null)
        {
            _selectedQuest.TargetCountChanged -= RefreshTargetText;
            _selectedQuest = null;
        }

        GetObject((int)GameObjects.QuestInfo).SetActive(false);
        GetButton((int)Buttons.CompleteButton).gameObject.SetActive(false);
        GetButton((int)Buttons.CancelButton).gameObject.SetActive(false);
        GetText((int)Texts.NOQuestText).gameObject.SetActive(true);
        foreach (Transform reward in GetRT((int)RectTransforms.QuestRewardSlots))
        {
            if (reward.gameObject == GetText((int)Texts.QuestRewardText).gameObject)
            {
                continue;
            }

            Managers.Resource.Destroy(reward.gameObject);
        }
    }

    private void LoadSaveData()
    {
        if (!Managers.Data.Load<JArray>(SaveKey, out var datas))
        {
            return;
        }

        foreach (var data in datas)
        {
            var saveData = data.ToObject<QuestUISaveData>();
            foreach (var element in _titleSubitems)
            {
                if (element.Key.Data.QuestID.Equals(saveData.QuestID))
                {
                    element.Value.ToggleQuestTracker(true);
                }
            }
        }
    }
}
