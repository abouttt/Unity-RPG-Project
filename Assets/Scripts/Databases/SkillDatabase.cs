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

    public SkillData FindSkillBy(string id)
    {
        return _skills.FirstOrDefault(skill => skill.SkillID.Equals(id));
    }

#if UNITY_EDITOR
    [ContextMenu("Find Skills")]
    public void FindSkills()
    {
        FindSkillsBy<SkillData>();
    }

    private void FindSkillsBy<T>() where T : SkillData
    {
        _skills = new();

        var guids = AssetDatabase.FindAssets($"t:{typeof(T)}");
        foreach (var guid in guids)
        {
            var assetPath = AssetDatabase.GUIDToAssetPath(guid);
            T skill = AssetDatabase.LoadAssetAtPath<T>(assetPath);

            _skills.Add(skill);
        }

        EditorUtility.SetDirty(this);
        AssetDatabase.SaveAssets();
    }
#endif
}
