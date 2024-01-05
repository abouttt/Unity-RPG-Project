using System;
using System.Collections.Generic;
using UnityEngine;
using AYellowpaper.SerializedCollections;
using Newtonsoft.Json.Linq;

public class SkillTree : MonoBehaviour
{
    [Serializable]
    public class SkillInit
    {
        public SkillData Data;
        [SerializeField, SerializedDictionary("자식 스킬", "레벨 조건")]
        public SerializedDictionary<SkillData, int> ChildrenData;
    }

    public static readonly string SaveKey = "SaveSkillTree";

    [SerializeField]
    private SkillInit[] _skillInits;
    private readonly List<Skill> _skills = new();
    private readonly List<Skill> _rootSkills = new();
    private readonly Dictionary<SkillData, Skill> _dataAndSkill = new();

    private void Awake()
    {
        InitSkills();
    }

    private void Start()
    {
        Player.Status.SkillPointChanged += CheckRootSkills;
        LoadSaveData();
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

        Player.Status.SkillPoint += totalSkillPoint;
    }

    public JArray GetSaveData()
    {
        var saveDatas = new JArray();

        foreach (var skill in _skills)
        {
            SkillSaveData saveData = new()
            {
                SkillID = skill.Data.SkillID,
                CurrentLevel = skill.CurrentLevel,
            };

            saveDatas.Add(JObject.FromObject(saveData));
        }

        return saveDatas;
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

    private void LoadSaveData()
    {
        if (!Managers.Data.Load<JArray>(SaveKey, out var datas))
        {
            return;
        }

        foreach (var data in datas)
        {
            var saveData = data.ToObject<SkillSaveData>();
            if (saveData.CurrentLevel == 0)
            {
                continue;
            }

            var skillData = SkillDatabase.GetInstance.FindSkillBy(saveData.SkillID);
            for (int i = 0; i < _skills.Count; i++)
            {
                if (_skills[i].Data.Equals(skillData))
                {
                    _skills[i].CurrentLevel = saveData.CurrentLevel;
                }
            }
        }
    }
}
