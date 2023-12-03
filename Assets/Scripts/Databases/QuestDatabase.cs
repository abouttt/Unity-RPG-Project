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

    public QuestData FindQuestBy(string questID) => _quests.FirstOrDefault(q => q.QuestID.Equals(questID));

    public List<QuestData> FindQuestsBy(string ownerID)
    {
        List<QuestData> result = new();
        if (_ownerQuests.TryGetValue(ownerID, out List<QuestData> quests))
        {
            foreach (var quest in quests)
            {
                result.Add(quest);
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
        string[] guids = AssetDatabase.FindAssets($"t:{typeof(T)}");
        foreach (string guid in guids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            var quest = AssetDatabase.LoadAssetAtPath<T>(assetPath);

            if (quest.GetType() == typeof(T))
            {
                _quests.Add(quest);
                if (!_ownerQuests.ContainsKey(quest.OwnerID))
                {
                    _ownerQuests.Add(quest.OwnerID, new());
                }
                _ownerQuests[quest.OwnerID].Add(quest);
            }

            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
        }
    }
#endif
}
