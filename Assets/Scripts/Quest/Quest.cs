using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Quest
{
    public QuestData Data { get; private set; }
    public NPC Owner { get; private set; }
    public NPC CompleteOwner { get; private set; }
    public QuestState State { get; private set; } = QuestState.Inactive;
    public IReadOnlyDictionary<QuestData.Target, int> Targets => _targets;
    public event Action TargetCountChanged;

    private readonly Dictionary<QuestData.Target, int> _targets = new();

    public Quest(NPC owner, QuestData data)
    {
        Data = data;
        Owner = owner;
        CompleteOwner = string.IsNullOrEmpty(Data.CompleteOwnerID) ? Owner : NPC.GetNPC(Data.CompleteOwnerID);
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

    //public Quest(QuestSaveData saveData)
    //{
    //    Data = QuestDatabase.GetInstance.FindQuestBy(saveData.QuestID);
    //    Owner = NPC.GetNPC(saveData.NPCID);
    //    CompleteOwner = string.IsNullOrEmpty(Data.CompleteOwnerID) ? Owner : NPC.GetNPC(Data.CompleteOwnerID);
    //    State = saveData.State;

    //    int i = 0;
    //    foreach (var target in Data.Targets)
    //    {
    //        _targets.Add(target, saveData.Counts[i++]);
    //    }

    //    Owner.Quests.Remove(Data);

    //    if (State is QuestState.Completable)
    //    {
    //        CompleteOwner.Quests.Add(Data);
    //    }
    //}

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

        CompleteOwner.RemoveQuest(Data);

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
        if (State is QuestState.Completable)
        {
            CompleteOwner.RemoveQuest(Data);
        }

        State = QuestState.Cancel;
        Owner.AddQuest(Data);
        TargetCountChanged = null;
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
                    CompleteOwner.RemoveQuest(Data);
                }

                return;
            }
        }

        if (prevState is not QuestState.Completable)
        {
            State = QuestState.Completable;
            CompleteOwner.AddQuest(Data);
        }
    }
}
