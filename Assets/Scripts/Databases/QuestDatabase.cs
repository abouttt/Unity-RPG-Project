using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using AYellowpaper.SerializedCollections;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(menuName = "Database/Quest Database", fileName = "QuestDatabase")]
public class QuestDatabase : SingletonScriptableObject<QuestDatabase>
{
    public IReadOnlyCollection<QuestData> Quests => _quests;

    [SerializeField]
    private List<QuestData> _quests;

    [SerializeField]
    private SerializedDictionary<string, List<QuestData>> _ownerQuests;

    public QuestData FindQuestBy(string questID)
    {
        return _quests.FirstOrDefault(quest => quest.QuestID.Equals(questID));
    }

    public List<QuestData> FindQuestsBy(string ownerID)
    {
        var result = new List<QuestData>();

        if (_ownerQuests.TryGetValue(ownerID, out var quests))
        {
            foreach (var questData in quests)
            {
                result.Add(questData);
            }
        }

        return result;
    }

#if UNITY_EDITOR
    [ContextMenu("Find Quests")]
    public void FindQuests()
    {
        FindQuestsBy<QuestData>();
    }

    private void FindQuestsBy<T>() where T : QuestData
    {
        _quests = new();
        _ownerQuests = new();

        var guids = AssetDatabase.FindAssets($"t:{typeof(T)}");
        foreach (var guid in guids)
        {
            var assetPath = AssetDatabase.GUIDToAssetPath(guid);
            T quest = AssetDatabase.LoadAssetAtPath<T>(assetPath);

            if (quest.GetType() == typeof(T))
            {
                _quests.Add(quest);

                if (!_ownerQuests.ContainsKey(quest.OwnerID))
                {
                    _ownerQuests.Add(quest.OwnerID, new());
                }

                _ownerQuests[quest.OwnerID].Add(quest);
            }
        }

        EditorUtility.SetDirty(this);
        AssetDatabase.SaveAssets();
    }
#endif
}
