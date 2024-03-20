using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

public class QuestManager : ISavable
{
    public static string SaveKey => "SaveQuest";

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

    public Quest Register(QuestData questData)
    {
        var newQuest = new Quest(questData);
        _activeQuests.Add(newQuest);
        NPC.TryRemoveQuestToNPC(questData.OwnerID, questData);
        QuestRegistered?.Invoke(newQuest);

        if (newQuest.CheckCompletable())
        {
            NPC.TryAddQuestToNPC(questData.CompleteOwnerID, questData);
            QuestCompletabled?.Invoke(newQuest);
        }

        return newQuest;
    }

    public void Unregister(Quest quest)
    {
        if (quest == null)
        {
            return;
        }

        if (_activeQuests.Remove(quest))
        {
            if (quest.State == QuestState.Completable)
            {
                NPC.TryRemoveQuestToNPC(quest.Data.CompleteOwnerID, quest.Data);
            }

            quest.Cancel();
            NPC.TryAddQuestToNPC(quest.Data.OwnerID, quest.Data);
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
                if (quest.State == QuestState.Completable)
                {
                    if (prevState != QuestState.Completable)
                    {
                        NPC.TryAddQuestToNPC(quest.Data.CompleteOwnerID, quest.Data);
                        QuestCompletabled?.Invoke(quest);
                    }
                }
                else if (prevState == QuestState.Completable)
                {
                    NPC.TryRemoveQuestToNPC(quest.Data.CompleteOwnerID, quest.Data);
                    QuestCompletableCanceled?.Invoke(quest);
                }
            }
        }
    }

    public void Complete(Quest quest)
    {
        if (quest == null)
        {
            return;
        }

        if (quest.Complete())
        {
            _activeQuests.Remove(quest);
            _completedQuests.Add(quest);
            NPC.TryRemoveQuestToNPC(quest.Data.CompleteOwnerID, quest.Data);
            QuestCompleted?.Invoke(quest);
        }
    }

    public Quest GetActiveQuest(QuestData questData)
    {
        return _activeQuests.Find(quest => quest.Data.Equals(questData));
    }

    public bool IsSameState(QuestData questData, QuestState questState)
    {
        var quest = GetActiveQuest(questData);
        if (quest != null)
        {
            return quest.State == questState;
        }

        return false;
    }

    public void Clear()
    {
        foreach (var quest in _activeQuests)
        {
            quest.Cancel();
        }

        _activeQuests.Clear();
        _completedQuests.Clear();

        QuestRegistered = null;
        QuestCompletabled = null;
        QuestCompletableCanceled = null;
        QuestCompleted = null;
        QuestUnRegistered = null;
    }

    public JArray CreateSaveData()
    {
        var saveData = new JArray();

        var questsSaveData = new JObject()
        {
            { ACTIVE_QUEST_SaveKey, CreateQuestsSaveData(_activeQuests) },
            { COMPLETE_QUEST_SaveKey, CreateQuestsSaveData(_completedQuests) },
        };

        saveData.Add(questsSaveData);
        return saveData;
    }

    public void LoadSaveData()
    {
        if (!Managers.Data.Load<JArray>(SaveKey, out var saveData))
        {
            return;
        }

        var questsSaveData = saveData[0].ToObject<JObject>();

        foreach (var element in questsSaveData)
        {
            var quests = questsSaveData[element.Key] as JArray;
            foreach (var data in quests)
            {
                var questSaveData = data.ToObject<QuestSaveData>();
                var quest = new Quest(questSaveData);
                NPC.TryRemoveQuestToNPC(quest.Data.OwnerID, quest.Data);

                if (quest.State is QuestState.Complete)
                {
                    _completedQuests.Add(quest);
                }
                else
                {
                    _activeQuests.Add(quest);
                    QuestRegistered?.Invoke(quest);

                    if (quest.CheckCompletable())
                    {
                        NPC.TryAddQuestToNPC(quest.Data.CompleteOwnerID, quest.Data);
                        QuestCompletabled?.Invoke(quest);
                    }
                }
            }
        }
    }

    private JArray CreateQuestsSaveData(List<Quest> quests)
    {
        var saveData = new JArray();

        foreach (var quest in quests)
        {
            var targets = new Dictionary<string, int>();
            foreach (var element in quest.Targets)
            {
                targets.Add(element.Key.TargetID, element.Value);
            }

            var questSaveData = new QuestSaveData()
            {
                QuestID = quest.Data.QuestID,
                State = quest.State,
                Targets = targets,
            };

            saveData.Add(JObject.FromObject(questSaveData));
        }

        return saveData;
    }
}
