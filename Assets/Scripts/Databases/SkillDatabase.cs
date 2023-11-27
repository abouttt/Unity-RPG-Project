using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(menuName = "Database/Skill Database", fileName = "SkillDatabase")]
public class SkillDatabase : SingletonScriptableObject<SkillDatabase>
{
    public IReadOnlyCollection<SkillData> Skills => _skills;

    [SerializeField]
    private List<SkillData> _skills;

    public SkillData FindSkillBy(string id) => _skills.FirstOrDefault(q => q.SkillID.Equals(id));

#if UNITY_EDITOR
    [ContextMenu("Find Skills")]
    private void FindSkills()
    {
        FindSkillsBy<SkillData>();
    }

    private void FindSkillsBy<T>() where T : SkillData
    {
        _skills = new List<SkillData>();
        string[] guids = AssetDatabase.FindAssets($"t:{typeof(T)}");
        foreach (string guid in guids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            var skill = AssetDatabase.LoadAssetAtPath<T>(assetPath);

            _skills.Add(skill);

            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
        }
    }
#endif
}
