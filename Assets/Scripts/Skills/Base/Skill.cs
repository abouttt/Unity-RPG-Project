using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class Skill : IUsable
{
    public SkillData Data { get; private set; }
    public bool IsLock { get; private set; } = true;
    public bool IsAcquirable { get; private set; } = false;
    public int CurrentLevel { get; set; }
    public IReadOnlyList<Skill> Parents => _parents;
    public IReadOnlyDictionary<Skill, int> Children => _children;
    public event Action SkillChanged;

    private readonly List<Skill> _parents = new();
    private readonly Dictionary<Skill, int> _children = new();

    public Skill(SkillData data)
    {
        Data = data;
    }

    public void LevelUp()
    {
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

    public void Check()
    {
        if (IsLock && CurrentLevel > 0)
        {
            IsLock = false;
            IsAcquirable = true;
            SkillChanged?.Invoke();
        }

        if (IsAcquirable)
        {
            foreach (var child in _children)
            {
                child.Key.Check();
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
        int skillPoint = 0;
        foreach (var element in _children)
        {
            skillPoint += element.Key.ResetSkill();
        }

        skillPoint += CurrentLevel;
        CurrentLevel = 0;
        IsLock = true;
        IsAcquirable = false;
        SkillChanged?.Invoke();
        Managers.Quest.ReceiveReport(Category.Skill, Data.SkillID, -1);

        return skillPoint;
    }

    public abstract bool Use();

    protected bool CheckCanUse()
    {
        if (Data.Cooldown.Current > 0)
        {
            return false;
        }

        if (Player.Status.HP < Data.RequiredHP ||
            Player.Status.MP < Data.RequiredMP ||
            Player.Status.SP < Data.RequiredSP)
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
