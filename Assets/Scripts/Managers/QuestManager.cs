using System;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager
{
    public event Action<Quest> QuestRegistered;
    public event Action<Quest> QuestCompletabled;
    public event Action<Quest> QuestCompletableCanceled;
    public event Action<Quest> QuestCompleted;
    public event Action<Quest> QuestUnRegistered;

    public IReadOnlyList<Quest> Quests => _quests;

    private readonly List<Quest> _quests = new();
    private readonly List<Quest> _completeQuests = new();

    public void Init()
    {

    }

    public Quest Register(NPC owner, QuestData questData)
    {
        if (IsRegistered(questData))
        {
            return null;
        }

        var newQuest = new Quest(owner, questData);
        _quests.Add(newQuest);
        newQuest.Owner.RemoveQuest(newQuest.Data);
        QuestRegistered?.Invoke(newQuest);

        newQuest.CheckCompletable();
        if (newQuest.State is QuestState.Completable)
        {
            QuestCompletabled?.Invoke(newQuest);
        }

        return newQuest;
    }

    public void Unregister(Quest quest)
    {
        if (quest is null)
        {
            return;
        }

        if (_quests.Remove(quest))
        {
            quest.Cancel();
            QuestUnRegistered?.Invoke(quest);
        }
    }

    public void ReceiveReport(Category category, string id, int count)
    {
        if (count == 0)
        {
            return;
        }

        foreach (var quest in _quests)
        {
            var prevState = quest.State;
            if (quest.ReceiveReport(category, id, count))
            {
                if (quest.State is QuestState.Completable)
                {
                    if (prevState is not QuestState.Completable)
                    {
                        QuestCompletabled?.Invoke(quest);
                    }
                }
                else if (prevState is QuestState.Completable)
                {
                    QuestCompletableCanceled?.Invoke(quest);
                }
            }
        }
    }

    public void Complete(Quest quest)
    {
        if (quest is null)
        {
            return;
        }

        if (quest.Complete())
        {
            _quests.Remove(quest);
            _completeQuests.Add(quest);
            QuestCompleted?.Invoke(quest);
        }
    }

    public Quest GetActiveQuest(QuestData questData)
    {
        return _quests.Find(quest => quest.Data.Equals(questData));
    }

    public bool IsRegistered(QuestData questData)
    {
        return GetActiveQuest(questData) is not null;
    }

    public bool IsCompletable(QuestData questData)
    {
        var quest = GetActiveQuest(questData);
        if (quest is null)
        {
            return false;
        }

        return quest.State is QuestState.Completable;
    }

    public void Clear()
    {
        _quests.Clear();
        _completeQuests.Clear();
        QuestRegistered = null;
        QuestCompletabled = null;
        QuestCompletableCanceled = null;
        QuestCompleted = null;
        QuestUnRegistered = null;
    }
}
