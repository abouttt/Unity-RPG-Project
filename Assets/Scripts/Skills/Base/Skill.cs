using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class Skill : IUsable
{
    public event Action SkillChanged;

    public SkillData Data { get; private set; }
    public bool IsLock { get; private set; } = true;
    public bool IsAcquirable { get; private set; } = false;
    public int CurrentLevel { get; set; }
    public IReadOnlyList<Skill> Parents => _parents;
    public IReadOnlyDictionary<Skill, int> Children => _children;

    private readonly List<Skill> _parents = new();
    private readonly Dictionary<Skill, int> _children = new();

    public Skill(SkillData data)
    {
        Data = data;
    }

    public abstract bool Use();

    public void LevelUp()
    {
        if (!IsAcquirable)
        {
            return;
        }

        if (IsLock)
        {
            IsLock = false;
            Managers.Quest.ReceiveReport(Category.Skill, Data.SkillID, 1);
        }

        CurrentLevel++;
        Player.Status.SkillPoint -= Data.RequiredSkillPoint;
        SkillChanged?.Invoke();
    }

    public void AddChild(Skill skill, int level)
    {
        if (_children.TryGetValue(skill, out _))
        {
            return;
        }

        _children.Add(skill, level);
        skill._parents.Add(this);
    }

    public void CheckState()
    {
        if (IsLock && CurrentLevel > 0)
        {
            IsLock = false;
            IsAcquirable = true;
            SkillChanged?.Invoke();
        }

        if (!IsLock)
        {
            foreach (var child in _children)
            {
                child.Key.CheckState();
            }
        }

        if (CurrentLevel == Data.MaxLevel)
        {
            return;
        }

        if (!IsAcquirable)
        {
            if (!CheckParentsLevel())
            {
                return;
            }
        }

        if (Player.Status.Level >= Data.RequiredPlayerLevel)
        {
            if (!IsAcquirable)
            {
                IsAcquirable = true;
            }

            SkillChanged?.Invoke();
        }
    }

    public int ResetSkill()
    {
        int skillPoint = CurrentLevel;

        if (!IsLock)
        {
            foreach (var element in _children)
            {
                skillPoint += element.Key.ResetSkill();
            }
        }

        bool prevAcquirable = IsAcquirable;
        CurrentLevel = 0;
        IsLock = true;
        IsAcquirable = false;

        if (prevAcquirable == true)
        {
            Managers.Quest.ReceiveReport(Category.Skill, Data.SkillID, -CurrentLevel);
            SkillChanged?.Invoke();
        }

        return skillPoint;
    }

    protected bool CanUse()
    {
        if (Data.SkillType is SkillType.Passive)
        {
            return false;
        }

        if (Data.Cooldown.Current > 0f)
        {
            return false;
        }

        if (Player.Status.HP < 0 ||
            Player.Status.MP < 0 ||
            Player.Status.SP < 0f)
        {
            return false;
        }

        Player.Status.HP -= Data.RequiredHP;
        Player.Status.MP -= Data.RequiredMP;
        Player.Status.SP -= Data.RequiredSP;

        Data.Cooldown.OnCooldowned();

        return true;
    }

    private bool CheckParentsLevel()
    {
        foreach (var parents in _parents)
        {
            if (parents.CurrentLevel < parents._children[this])
            {
                return false;
            }
        }

        return true;
    }
}
