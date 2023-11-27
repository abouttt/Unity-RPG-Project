using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(menuName = "Database/Quest Database", fileName = "QuestDatabase")]
public class QuestDatabase : SingletonScriptableObject<QuestDatabase>
{
    public IReadOnlyCollection<QuestData> Quests => _quests;

    [SerializeField]
    private List<QuestData> _quests;

    public QuestData FindQuestBy(string id) => _quests.FirstOrDefault(q => q.QuestID.Equals(id));

#if UNITY_EDITOR
    [ContextMenu("Find Quests")]
    public void FindQuests()
    {
        FindQuestsBy<QuestData>();
    }

    private void FindQuestsBy<T>() where T : QuestData
    {
        _quests = new List<QuestData>();
        string[] guids = AssetDatabase.FindAssets($"t:{typeof(T)}");
        foreach (string guid in guids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            var quest = AssetDatabase.LoadAssetAtPath<T>(assetPath);

            if (quest.GetType() == typeof(T))
            {
                _quests.Add(quest);
            }

            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
        }
    }
#endif
}
