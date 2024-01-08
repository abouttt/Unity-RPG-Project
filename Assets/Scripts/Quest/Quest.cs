using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Quest
{
    public QuestData Data { get; private set; }
    public NPC Owner => NPC.GetNPC(Data.OwnerID);
    public NPC CompleteOwner => NPC.GetNPC(Data.CompleteOwnerID);
    public QuestState State { get; private set; } = QuestState.Inactive;
    public IReadOnlyDictionary<QuestData.Target, int> Targets => _targets;
    public event Action TargetCountChanged;

    private readonly Dictionary<QuestData.Target, int> _targets = new();

    public Quest(QuestData data)
    {
        Data = data;
        State = QuestState.Active;
        foreach (var target in Data.Targets)
        {
            int count = 0;
            if (target.Category is Category.Item)
            {
                count = Player.ItemInventory.GetSameItemCount(target.TargetID);
            }

            _targets.Add(target, count);
        }
    }

    public Quest(QuestSaveData saveData)
    {
        Data = QuestDatabase.GetInstance.FindQuestBy(saveData.QuestID);
        State = saveData.State;

        int i = 0;
        foreach (var target in Data.Targets)
        {
            _targets.Add(target, saveData.Counts[i++]);
        }
    }

    public void AddQuestToOwner()
    {
        var npc = Owner;
        if (npc != null)
        {
            npc.AddQuest(Data);
        }
    }

    public void RemoveQuestFromOwner()
    {
        var owner = Owner;
        if (owner != null)
        {
            owner.RemoveQuest(Data);
        }
    }

    public void AddQuestToCompletableOwner()
    {
        var completeOwner = CompleteOwner;
        if (completeOwner != null)
        {
            completeOwner.AddQuest(Data);
        }
    }

    public void RemoveQuestFromCompletableOwner()
    {
        var completeOwner = CompleteOwner;
        if (completeOwner != null)
        {
            completeOwner.RemoveQuest(Data);
        }
    }

    public bool ReceiveReport(Category category, string id, int count)
    {
        if (State is QuestState.Complete)
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
            var target = element.Key;

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
        if (State is not QuestState.Completable)
        {
            return false;
        }

        State = QuestState.Complete;
        Player.Status.Gold += Data.RewardGold;
        Player.Status.XP += Data.RewardXP;

        foreach (var element in Data.RewardItems.ToList())
        {
            Player.ItemInventory.AddItem(element.Key, element.Value);
        }

        foreach (var element in _targets.ToList())
        {
            var target = element.Key;
            if (target.Category is not Category.Item || !target.RemoveAfterCompletion)
            {
                continue;
            }

            Player.ItemInventory.RemoveItem(element.Key.TargetID, element.Key.CompleteCount);
        }

        TargetCountChanged = null;
        Managers.Quest.ReceiveReport(Category.Quest, Data.QuestID, 1);

        return true;
    }

    public void Cancel()
    {
        Clear();
        Managers.Quest.ReceiveReport(Category.Quest, Data.QuestID, -1);
    }

    public void CheckCompletable()
    {
        var prevState = State;
        foreach (var element in _targets)
        {
            if (element.Key.CompleteCount > element.Value)
            {
                if (prevState is QuestState.Completable)
                {
                    State = QuestState.Active;
                }

                return;
            }
        }

        if (prevState is not QuestState.Completable)
        {
            State = QuestState.Completable;
        }
    }

    public void Clear()
    {
        State = QuestState.Inactive;
        TargetCountChanged = null;
    }
}
