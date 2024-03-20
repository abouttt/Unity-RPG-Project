using System;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;
using AYellowpaper.SerializedCollections;

public class SkillTree : MonoBehaviour, ISavable
{
    [Serializable]
    public class SkillInit
    {
        public SkillData Data;
        [SerializeField, SerializedDictionary("자식 스킬", "레벨 조건")]
        public SerializedDictionary<SkillData, int> ChildrenData;
    }

    public static string SaveKey => "SaveSkillTree";

    [SerializeField]
    private SkillInit[] _skillInits;
    private readonly List<Skill> _rootSkills = new();
    private readonly Dictionary<SkillData, Skill> _skills = new();

    private void Awake()
    {
        InitSkills();
        LoadSaveData();
    }

    private void Start()
    {
        Player.Status.SkillPointChanged += CheckRootSkills;
        CheckRootSkills();
    }

    public void CheckRootSkills()
    {
        foreach (var skill in _rootSkills)
        {
            skill.CheckState();
        }
    }

    public Skill GetSkillBy(SkillData skillData)
    {
        if (_skills.TryGetValue(skillData, out var skill))
        {
            return skill;
        }

        return null;
    }

    public Skill GetSkillBy(string id)
    {
        foreach (var skill in _skills)
        {
            if (skill.Key.SkillID.Equals(id))
            {
                return skill.Value;
            }
        }

        return null;
    }

    public void ResetSkills()
    {
        int totalSkillPoint = 0;
        foreach (var skill in _rootSkills)
        {
            totalSkillPoint = skill.ResetSkill();
        }

        Player.Status.SkillPoint += totalSkillPoint;
    }

    private void InitSkills()
    {
        // 스킬 생성
        foreach (var skillInit in _skillInits)
        {
            _skills.Add(skillInit.Data, skillInit.Data.CreateSkill());
        }

        // 자식 스킬 설정
        foreach (var skillInit in _skillInits)
        {
            foreach (var childData in skillInit.ChildrenData)
            {
                _skills[skillInit.Data].AddChild(_skills[childData.Key], childData.Value);
            }
        }

        // 루트 스킬 설정
        foreach (var element in _skills)
        {
            if (element.Key.Root)
            {
                _rootSkills.Add(element.Value);
            }
        }
    }

    public JArray CreateSaveData()
    {
        var saveData = new JArray();

        foreach (var element in _skills)
        {
            var skillSaveData = new SkillSaveData()
            {
                SkillID = element.Key.SkillID,
                CurrentLevel = element.Value.CurrentLevel,
            };

            saveData.Add(JObject.FromObject(skillSaveData));
        }

        return saveData;
    }

    public void LoadSaveData()
    {
        if (!Managers.Data.Load<JArray>(SaveKey, out var saveData))
        {
            return;
        }

        foreach (var token in saveData)
        {
            var skillSaveData = token.ToObject<SkillSaveData>();
            if (skillSaveData.CurrentLevel == 0)
            {
                continue;
            }

            var skillData = SkillDatabase.GetInstance.FindSkillBy(skillSaveData.SkillID);
            _skills[skillData].CurrentLevel = skillSaveData.CurrentLevel;
        }
    }
}
