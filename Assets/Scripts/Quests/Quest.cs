using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Quest
{
    public event Action TargetCountChanged;
    public QuestData Data { get; private set; }
    public QuestState State { get; private set; } = QuestState.Inactive;
    public IReadOnlyDictionary<QuestTarget, int> Targets => _targets;

    private readonly Dictionary<QuestTarget, int> _targets = new();

    public Quest(QuestData questData)
    {
        Data = questData;
        State = QuestState.Active;

        foreach (var target in Data.Targets)
        {
            int count = 0;

            switch (target.Category)
            {
                case Category.Item:
                    count = Player.ItemInventory.GetSameItemCount(target.TargetID);
                    break;
                case Category.Skill:
                    count = Player.SkillTree.GetSkillBy(target.TargetID).CurrentLevel;
                    break;
            }

            _targets.Add(target, count);
        }
    }

    public Quest(QuestSaveData saveData)
    {
        Data = QuestDatabase.GetInstance.FindQuestBy(saveData.QuestID);
        State = saveData.State;

        foreach (var target in Data.Targets)
        {
            foreach (var element in saveData.Targets)
            {
                if (target.TargetID.Equals(element.Key))
                {
                    _targets.Add(target, element.Value);
                    saveData.Targets.Remove(element.Key);
                    break;
                }
            }
        }
    }

    public bool ReceiveReport(Category category, string id, int count)
    {
        if (State == QuestState.Complete)
        {
            return false;
        }

        if (count == 0)
        {
            return false;
        }

        bool isChanged = false;

        foreach (var element in _targets.ToList())
        {
            QuestTarget target = element.Key;

            if (target.Category != category)
            {
                continue;
            }

            if (!target.TargetID.Equals(id))
            {
                continue;
            }

            _targets[target] = element.Value + count;
            isChanged = true;
        }

        if (isChanged)
        {
            CheckCompletable();
            TargetCountChanged?.Invoke();
        }

        return isChanged;
    }

    public bool Complete()
    {
        if (State != QuestState.Completable)
        {
            return false;
        }

        State = QuestState.Complete;
        TargetCountChanged = null;

        Player.Status.Gold += Data.RewardGold;
        Player.Status.XP += Data.RewardXP;

        foreach (var element in Data.RewardItems)
        {
            Player.ItemInventory.AddItem(element.Key, element.Value);
        }

        foreach (var element in _targets)
        {
            var target = element.Key;
            if (target.Category != Category.Item || !target.RemoveAfterCompletion)
            {
                continue;
            }

            Player.ItemInventory.RemoveItem(element.Key.TargetID, element.Key.CompleteCount);
        }

        Managers.Quest.ReceiveReport(Category.Quest, Data.QuestID, 1);

        return true;
    }

    public void Cancel()
    {
        State = QuestState.Inactive;
        TargetCountChanged = null;
        Managers.Quest.ReceiveReport(Category.Quest, Data.QuestID, -1);
    }

    public bool CheckCompletable()
    {
        foreach (var element in _targets)
        {
            if (element.Key.CompleteCount > element.Value)
            {
                State = QuestState.Active;
                return false;
            }
        }

        State = QuestState.Completable;
        return true;
    }
}
