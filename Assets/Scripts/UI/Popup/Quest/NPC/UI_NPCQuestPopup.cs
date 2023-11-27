using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class UI_NPCQuestPopup : UI_Popup
{
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
        AcceptButton,
    }

    enum Texts
    {
        QuestTitleText,
        QuestDescriptionText,
        QuestTargetText,
        QuestRewardText,
        NOQuestText,
    }

    private readonly Dictionary<QuestData, UI_NPCQuestTitleSubitem> _titleSubitems = new();
    private readonly StringBuilder _sb = new(50);
    private QuestData _selectedQuestData;
    private NPC _npc;

    protected override void Init()
    {
        base.Init();

        BindObject(typeof(GameObjects));
        BindRT(typeof(RectTransforms));
        BindButton(typeof(Buttons));
        BindText(typeof(Texts));

        GetButton((int)Buttons.CloseButton).onClick.AddListener(Managers.UI.Close<UI_NPCQuestPopup>);
        GetButton((int)Buttons.CompleteButton).onClick.AddListener(() =>
        {
            Managers.Quest.Complete(Managers.Quest.GetActiveQuest(_selectedQuestData));
            SelectedQuestDataCleanup();
        });

        GetButton((int)Buttons.AcceptButton).onClick.AddListener(() =>
        {
            var quest = Managers.Quest.Register(_npc, _selectedQuestData);
            if (quest.State == QuestState.Completable)
            {
                _titleSubitems[_selectedQuestData].ToggleCompleteText(true);
                Clear();
            }
            else
            {
                SelectedQuestDataCleanup();
            }
        });

        Player.Status.LevelChanged += () =>
        {
            if (_npc != null)
            {
                SetNPCQuest(_npc);
            }
        };

        Clear();
    }

    private void Start()
    {
        Managers.UI.Register<UI_NPCQuestPopup>(this);

        Closed += () =>
        {
            foreach (var element in _titleSubitems.ToList())
            {
                Managers.Resource.Destroy(element.Value.gameObject);
            }
            _titleSubitems.Clear();
            Clear();
            _npc = null;
            Managers.UI.Get<UI_NPCMenuPopup>().ToggleMenu(true);
        };
    }

    public void SetNPCQuest(NPC npc)
    {
        _npc = npc;

        foreach (var questData in npc.Quests)
        {
            if (Player.Status.Level < questData.LimitLevel)
            {
                continue;
            }

            if (_titleSubitems.TryGetValue(questData, out var _))
            {
                continue;
            }

            var go = Managers.Resource.Instantiate("UI_NPCQuestTitleSubitem", GetRT((int)RectTransforms.QuestTitleSubitems), true);
            var questTitleSubitem = go.GetComponent<UI_NPCQuestTitleSubitem>();
            questTitleSubitem.SetQuestTitle(questData);
            questTitleSubitem.ToggleCompleteText(Managers.Quest.IsCompletable(questData));
            _titleSubitems.Add(questData, questTitleSubitem);
        }

        GetText((int)Texts.NOQuestText).gameObject.SetActive(_titleSubitems.Count == 0);
    }

    public void SetQuestDescription(QuestData questData)
    {
        if (_selectedQuestData != null && _selectedQuestData.Equals(questData))
        {
            return;
        }

        Clear();
        _selectedQuestData = questData;
        GetObject((int)GameObjects.QuestInfo).SetActive(true);
        GetText((int)Texts.QuestTitleText).text = questData.QuestName;
        GetText((int)Texts.QuestDescriptionText).text = questData.Description;
        if (Managers.Quest.IsCompletable(questData))
        {
            RefreshTargetText(questData, true);
            GetButton((int)Buttons.CompleteButton).gameObject.SetActive(true);
        }
        else
        {
            RefreshTargetText(questData, false);
            GetButton((int)Buttons.AcceptButton).gameObject.SetActive(true);
        }
        SetRewardText(questData);
    }

    private void RefreshTargetText(QuestData questData, bool showCurrentCount)
    {
        _sb.Clear();
        _sb.AppendLine("[목적]");

        foreach (var target in questData.Targets)
        {
            if (showCurrentCount)
            {
                var quest = Managers.Quest.GetActiveQuest(questData);
                var completeCount = target.CompleteCount;
                var currentCount = Mathf.Clamp(quest.Targets[target], 0, completeCount);
                _sb.AppendLine($"{target.Description} ({currentCount}/{completeCount})");
            }
            else
            {
                _sb.AppendLine($"{target.Description} (0/{target.CompleteCount})");
            }
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

    private void SelectedQuestDataCleanup()
    {
        Managers.Resource.Destroy(_titleSubitems[_selectedQuestData].gameObject);
        _titleSubitems.Remove(_selectedQuestData);
        Clear();
        GetText((int)Texts.NOQuestText).gameObject.SetActive(_titleSubitems.Count == 0);
    }

    private void Clear()
    {
        _selectedQuestData = null;
        GetObject((int)GameObjects.QuestInfo).SetActive(false);
        GetButton((int)Buttons.CompleteButton).gameObject.SetActive(false);
        GetButton((int)Buttons.AcceptButton).gameObject.SetActive(false);
        GetText((int)Texts.NOQuestText).gameObject.SetActive(false);
        foreach (Transform reward in GetRT((int)RectTransforms.QuestRewardSlots))
        {
            if (reward.gameObject == GetText((int)Texts.QuestRewardText).gameObject)
            {
                continue;
            }

            Managers.Resource.Destroy(reward.gameObject);
        }
    }
}
