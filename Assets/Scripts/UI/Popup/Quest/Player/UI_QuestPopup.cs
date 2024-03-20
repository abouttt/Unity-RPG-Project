using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Newtonsoft.Json.Linq;

public class UI_QuestPopup : UI_Popup, ISavable
{
    public static string SaveKey => "SaveQuestUI";

    enum GameObjects
    {
        QuestInfo,
    }

    enum RectTransforms
    {
        QuestTitleSubitems,
        QuestRewardSlots,
    }

    enum Texts
    {
        QuestTitleText,
        QuestDescriptionText,
        QuestTargetText,
        QuestRewardText,
        NOQuestText,
    }

    enum Buttons
    {
        CloseButton,
        CompleteButton,
        CancelButton,
    }

    private Quest _selectedQuest;
    private readonly Dictionary<Quest, UI_QuestTitleSubitem> _titleSubitems = new();
    private readonly StringBuilder _sb = new(50);

    protected override void Init()
    {
        base.Init();

        BindObject(typeof(GameObjects));
        BindRT(typeof(RectTransforms));
        BindText(typeof(Texts));
        BindButton(typeof(Buttons));

        GetButton((int)Buttons.CloseButton).onClick.AddListener(Managers.UI.Close<UI_QuestPopup>);
        GetButton((int)Buttons.CompleteButton).onClick.AddListener(() => Managers.Quest.Complete(_selectedQuest));
        GetButton((int)Buttons.CancelButton).onClick.AddListener(() => Managers.Quest.Unregister(_selectedQuest));

        Managers.Quest.QuestRegistered += OnQuestRegisterd;
        Managers.Quest.QuestCompletabled += OnQuestCompletabled;
        Managers.Quest.QuestCompletableCanceled += OnQuestCompletableCanceld;
        Managers.Quest.QuestCompleted += OnQuestCompletedOrCanceled;
        Managers.Quest.QuestUnRegistered += OnQuestCompletedOrCanceled;

        Clear();
    }

    private void Start()
    {
        Managers.UI.Register<UI_QuestPopup>(this);
        LoadSaveData();
    }

    public void SetQuestDescription(Quest quest)
    {
        if (quest == null)
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
        ToggleCompleteButton(quest, quest.State == QuestState.Completable);
        GetText((int)Texts.NOQuestText).gameObject.SetActive(false);
        GetButton((int)Buttons.CancelButton).gameObject.SetActive(true);
    }

    public void ToggleQuestTracker(Quest quest, bool toggle)
    {
        if (_titleSubitems.TryGetValue(quest, out var subitem))
        {
            subitem.ToggleQuestTracker(toggle);
        }
    }

    private void OnQuestRegisterd(Quest quest)
    {
        var go = Managers.Resource.Instantiate("UI_QuestTitleSubitem.prefab", GetRT((int)RectTransforms.QuestTitleSubitems), true);
        var subitem = go.GetComponent<UI_QuestTitleSubitem>();
        subitem.SetQuestTitle(quest);
        _titleSubitems.Add(quest, subitem);
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
            int completeCount = target.Key.CompleteCount;
            int currentCount = Mathf.Clamp(target.Value, 0, completeCount);
            _sb.AppendLine($"- {target.Key.Description} ({currentCount}/{completeCount})");
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
            var go = Managers.Resource.Instantiate("UI_QuestRewardSlot.prefab", GetRT((int)RectTransforms.QuestRewardSlots), true);
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
        if (_selectedQuest != null)
        {
            _selectedQuest.TargetCountChanged -= RefreshTargetText;
            _selectedQuest = null;
        }

        GetObject((int)GameObjects.QuestInfo).SetActive(false);
        GetText((int)Texts.NOQuestText).gameObject.SetActive(true);
        GetButton((int)Buttons.CompleteButton).gameObject.SetActive(false);
        GetButton((int)Buttons.CancelButton).gameObject.SetActive(false);

        foreach (Transform reward in GetRT((int)RectTransforms.QuestRewardSlots))
        {
            if (reward.gameObject == GetText((int)Texts.QuestRewardText).gameObject)
            {
                continue;
            }

            Managers.Resource.Destroy(reward.gameObject);
        }
    }

    public JArray CreateSaveData()
    {
        var saveData = new JArray();

        foreach (var element in _titleSubitems)
        {
            if (!element.Value.IsShowedTracker())
            {
                continue;
            }

            var questUISaveData = new QuestUISaveData()
            {
                QuestID = element.Key.Data.QuestID
            };

            saveData.Add(JObject.FromObject(questUISaveData));
        }

        return saveData;
    }

    public void LoadSaveData()
    {
        if (!Managers.Data.Load<JArray>(SaveKey, out var saveData))
        {
            return;
        }

        foreach (var token in saveData)
        {
            var questUISaveData = token.ToObject<QuestUISaveData>();
            foreach (var element in _titleSubitems)
            {
                if (element.Key.Data.QuestID.Equals(questUISaveData.QuestID))
                {
                    element.Value.ToggleQuestTracker(true);
                }
            }
        }
    }
}
