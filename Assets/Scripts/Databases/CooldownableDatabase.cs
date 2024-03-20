using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(menuName = "Database/Cooldownable Database", fileName = "CooldownableDatabase")]
public class CooldownableDatabase : SingletonScriptableObject<CooldownableDatabase>
{
    public IReadOnlyList<ItemData> CooldownableItems => _cooldownableItems;
    public IReadOnlyList<SkillData> CooldownableSkills => _cooldownableSkills;

    [SerializeField]
    private List<ItemData> _cooldownableItems;

    [SerializeField]
    private List<SkillData> _cooldownableSkills;

#if UNITY_EDITOR
    [ContextMenu("Find Cooldown")]
    public void FindCooldownable()
    {
        FindCooldownable<ICooldownable>();
    }

    private void FindCooldownable<T>() where T : ICooldownable
    {
        _cooldownableItems = new();
        foreach (var itemData in ItemDatabase.GetInstance.Items)
        {
            if (itemData is ICooldownable)
            {
                _cooldownableItems.Add(itemData);
            }
        }

        _cooldownableSkills = new();
        foreach (var skillData in SkillDatabase.GetInstance.Skills)
        {
            if (skillData is ICooldownable)
            {
                _cooldownableSkills.Add(skillData);
            }
        }

        EditorUtility.SetDirty(this);
        AssetDatabase.SaveAssets();
    }
#endif
}
