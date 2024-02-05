using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Newtonsoft.Json.Linq;

public class QuestManager
{
    public static readonly string SaveKey = "SaveQuest";

    private const string ACTIVE_QUEST_SaveKey = "SaveActiveQuest";
    private const string COMPLETE_QUEST_SaveKey = "SaveCompleteQuest";

    public event Action<Quest> QuestRegistered;
    public event Action<Quest> QuestCompletabled;
    public event Action<Quest> QuestCompletableCanceled;
    public event Action<Quest> QuestCompleted;
    public event Action<Quest> QuestUnRegistered;

    public IReadOnlyList<Quest> ActiveQuests => _activeQuests;
    public IReadOnlyList<Quest> CompleteQuests => _completedQuests;

    private readonly List<Quest> _activeQuests = new();
    private readonly List<Quest> _completedQuests = new();
    private bool _isSaveLoadCleanup;

    public void Init()
    {
        LoadSaveData();
    }

    public Quest Register(QuestData questData)
    {
        if (GetActiveQuest(questData) is not null)
        {
            return null;
        }

        var newQuest = new Quest(questData);
        _activeQuests.Add(newQuest);
        newQuest.RemoveQuestFromOwner();
        QuestRegistered?.Invoke(newQuest);

        newQuest.CheckCompletable();
        if (newQuest.State is QuestState.Completable)
        {
            newQuest.AddQuestToCompletableOwner();
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

        if (_activeQuests.Remove(quest))
        {
            if (quest.State is QuestState.Completable)
            {
                quest.RemoveQuestFromCompletableOwner();
            }
            quest.Cancel();
            quest.AddQuestToOwner();
            QuestUnRegistered?.Invoke(quest);
        }
    }

    public void ReceiveReport(Category category, string id, int count)
    {
        if (count == 0)
        {
            return;
        }

        foreach (var quest in _activeQuests)
        {
            var prevState = quest.State;
            if (quest.ReceiveReport(category, id, count))
            {
                if (quest.State is QuestState.Completable)
                {
                    if (prevState is not QuestState.Completable)
                    {
                        quest.AddQuestToCompletableOwner();
                        QuestCompletabled?.Invoke(quest);
                    }
                }
                else if (prevState is QuestState.Completable)
                {
                    quest.RemoveQuestFromCompletableOwner();
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
            _activeQuests.Remove(quest);
            _completedQuests.Add(quest);
            quest.RemoveQuestFromCompletableOwner();
            QuestCompleted?.Invoke(quest);
        }
    }

    public Quest GetActiveQuest(QuestData questData)
    {
        return _activeQuests.Find(quest => quest.Data.Equals(questData));
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

    public void SaveLoadCleanup()
    {
        if (_isSaveLoadCleanup)
        {
            return;
        }

        foreach (var quest in _activeQuests)
        {
            quest.RemoveQuestFromOwner();
            QuestRegistered?.Invoke(quest);
            if (quest.State is QuestState.Completable)
            {
                quest.AddQuestToCompletableOwner();
                QuestCompletabled?.Invoke(quest);
            }
        }

        foreach (var quest in _completedQuests)
        {
            quest.RemoveQuestFromOwner();
        }

        _isSaveLoadCleanup = true;
    }

    public void Clear()
    {
        foreach (var quest in _activeQuests)
        {
            quest.Clear();
        }
        _activeQuests.Clear();
        _completedQuests.Clear();
        QuestRegistered = null;
        QuestCompletabled = null;
        QuestCompletableCanceled = null;
        QuestCompleted = null;
        QuestUnRegistered = null;
        _isSaveLoadCleanup = false;
    }

    public JObject GetSaveData()
    {
        return new JObject
        {
            { ACTIVE_QUEST_SaveKey, CreateSaveData(_activeQuests) },
            { COMPLETE_QUEST_SaveKey, CreateSaveData(_completedQuests) },
        };
    }

    private JArray CreateSaveData(IReadOnlyList<Quest> quests)
    {
        var saveDatas = new JArray();

        foreach (var quest in quests)
        {
            QuestSaveData saveData = new()
            {
                QuestID = quest.Data.QuestID,
                State = quest.State,
                Counts = quest.Targets.Select(t => t.Value).ToArray(),
            };

            saveDatas.Add(JObject.FromObject(saveData));
        }

        return saveDatas;
    }

    private void LoadSaveData()
    {
        if (!Managers.Data.Load<JObject>(SaveKey, out var root))
        {
            return;
        }

        foreach (var element in root)
        {
            var datas = root[element.Key] as JArray;

            foreach (var data in datas)
            {
                var saveData = data.ToObject<QuestSaveData>();
                var quest = new Quest(saveData);
                if (quest.State is QuestState.Complete)
                {
                    _completedQuests.Add(quest);
                }
                else
                {
                    _activeQuests.Add(quest);
                    quest.CheckCompletable();
                }
            }
        }
    }
}
