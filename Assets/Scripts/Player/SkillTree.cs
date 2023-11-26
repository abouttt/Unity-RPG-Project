using System;
using System.Collections.Generic;
using UnityEngine;
using AYellowpaper.SerializedCollections;

public class SkillTree : MonoBehaviour
{
    #region SkillInit
    [Serializable]
    public class SkillInit
    {
        public SkillData Data;
        [SerializeField, SerializedDictionary("자식 스킬", "레벨 조건")]
        public SerializedDictionary<SkillData, int> ChildrenData;
    }
    #endregion

    public int SkillPoint 
    {
        get => _skillPoint;
        set
        {
            _skillPoint = value;
            CheckRootSkills();
            SkillPointChanged?.Invoke();
        }
    }

    public event Action SkillPointChanged;

    [SerializeField]
    private SkillInit[] _skillInits;

    private readonly List<Skill> _skills = new();
    private readonly List<Skill> _rootSkills = new();
    private readonly Dictionary<SkillData, Skill> _dataAndSkill = new();
    private int _skillPoint = 0;

    private void Awake()
    {
        InitSkills();
    }

    private void Start()
    {
        CheckRootSkills();
    }

    public void CheckRootSkills()
    {
        foreach (var skill in _rootSkills)
        {
            skill.Check();
        }
    }

    public Skill GetSkill(SkillData skillData)
    {
        if (_dataAndSkill.TryGetValue(skillData, out var skill))
        {
            return skill;
        }

        return null;
    }

    public void ResetSkills()
    {
        var totalSkillPoint = 0;
        foreach (var skill in _rootSkills)
        {
            totalSkillPoint = skill.ResetSkill();
        }

        SkillPoint += totalSkillPoint;
    }

    private void InitSkills()
    {
        // 스킬 생성
        foreach (var skillInit in _skillInits)
        {
            var skill = skillInit.Data.CreateSkill();
            _skills.Add(skill);
            _dataAndSkill.Add(skillInit.Data, skill);
        }

        // 자식 스킬 설정
        foreach (var skillInit in _skillInits)
        {
            foreach (var childData in skillInit.ChildrenData)
            {
                var skill = _dataAndSkill[skillInit.Data];
                skill.AddChild(_dataAndSkill[childData.Key], childData.Value);
            }
        }

        // 루트 스킬 설정
        foreach (var skill in _skills)
        {
            if (skill.Data.Root)
            {
                _rootSkills.Add(skill);
            }
        }
    }
}
