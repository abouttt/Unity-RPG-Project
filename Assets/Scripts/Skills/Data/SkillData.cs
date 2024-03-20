using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Skill/Skill Data", fileName = "Skill_")]
public class SkillData : ScriptableObject, ICooldownable
{
    [field: SerializeField]
    public string SkillID { get; private set; }

    [field: SerializeField]
    public string SkillName { get; private set; }

    [field: SerializeField]
    public Sprite SkillImage { get; private set; }

    [field: SerializeField]
    public Sprite SkillFrame { get; private set; }

    [field: SerializeField]
    public SkillType SkillType { get; private set; }

    [field: SerializeField, TextArea]
    public string Description { get; private set; }

    [field: SerializeField]
    public int MaxLevel { get; private set; }

    [field: SerializeField]
    public int RequiredPlayerLevel { get; private set; }

    [field: SerializeField]
    public int RequiredSkillPoint { get; private set; }

    [field: SerializeField]
    public int RequiredHP { get; private set; }

    [field: SerializeField]
    public int RequiredMP { get; private set; }

    [field: SerializeField]
    public int RequiredSP { get; private set; }

    [field: SerializeField]
    public string SkillClassName { get; private set; }

    [field: SerializeField]
    public bool Root { get; private set; }

    [field: SerializeField]
    public Cooldown Cooldown { get; set; }

    public Skill CreateSkill()
    {
        var skill = GetInstance();
        if (skill == null)
        {
            Debug.Log($"{SkillName} class name no exist");
            return null;
        }

        return skill;
    }

    private Skill GetInstance()
    {
        var type = Type.GetType(SkillClassName);
        if (type != null)
        {
            return (Skill)Activator.CreateInstance(type, new object[] { this });
        }

        return null;
    }

    public bool Equals(SkillData other)
    {
        if (other == null)
        {
            return false;
        }

        if (GetType() != other.GetType())
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return SkillID.Equals(other.SkillID);
    }
}
