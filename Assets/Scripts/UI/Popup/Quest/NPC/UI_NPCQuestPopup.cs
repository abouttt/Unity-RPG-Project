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
        AcceptButton,
    }

    private NPC _npc;
    private QuestData _selectedQuestData;
    private readonly Dictionary<QuestData, UI_NPCQuestTitleSubitem> _titleSubitems = new();
    private readonly StringBuilder _sb = new(50);

    protected override void Init()
    {
        base.Init();

        BindObject(typeof(GameObjects));
        BindRT(typeof(RectTransforms));
        BindText(typeof(Texts));
        BindButton(typeof(Buttons));

        GetButton((int)Buttons.CloseButton).onClick.AddListener(Managers.UI.Close<UI_NPCQuestPopup>);
        GetButton((int)Buttons.CompleteButton).onClick.AddListener(() =>
        {
            Managers.Quest.Complete(Managers.Quest.GetActiveQuest(_selectedQuestData));
            CleanupSelectedQuestData();
        });

        GetButton((int)Buttons.AcceptButton).onClick.AddListener(() =>
        {
            var quest = Managers.Quest.Register(_selectedQuestData);
            if (quest.State == QuestState.Completable)
            {
                _titleSubitems[_selectedQuestData].ToggleCompleteText(true);
                Clear();
            }
            else
            {
                CleanupSelectedQuestData();
            }
        });

        Player.Status.LevelChanged += () =>
        {
            if (_npc != null)
            {
                SetNPC(_npc);
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
            _npc = null;
            Clear();
            Managers.UI.Get<UI_NPCMenuPopup>().PopupRT.gameObject.SetActive(true);
        };
    }

    public void SetNPC(NPC npc)
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

            var go = Managers.Resource.Instantiate("UI_NPCQuestTitleSubitem.prefab", GetRT((int)RectTransforms.QuestTitleSubitems), true);
            var subitem = go.GetComponent<UI_NPCQuestTitleSubitem>();
            subitem.SetQuestTitle(questData);
            _titleSubitems.Add(questData, subitem);
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

        if (Managers.Quest.IsSameState(questData, QuestState.Completable))
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
                int completeCount = target.CompleteCount;
                int currentCount = Mathf.Clamp(Managers.Quest.GetActiveQuest(questData).Targets[target], 0, completeCount);
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
            var go = Managers.Resource.Instantiate("UI_QuestRewardSlot.prefab", GetRT((int)RectTransforms.QuestRewardSlots), true);
            go.GetComponent<UI_QuestRewardSlot>().SetReward(element.Key, element.Value);
        }
    }

    private void CleanupSelectedQuestData()
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
        GetText((int)Texts.NOQuestText).gameObject.SetActive(false);
        GetButton((int)Buttons.CompleteButton).gameObject.SetActive(false);
        GetButton((int)Buttons.AcceptButton).gameObject.SetActive(false);

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
