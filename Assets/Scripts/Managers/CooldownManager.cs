using System.Collections.Generic;
using UnityEngine;

public class CooldownManager
{
    private readonly HashSet<Cooldown> _cooldowns = new();
    private readonly Queue<Cooldown> _cooldownCompleteQueue = new();

    public void Cooling()
    {
        foreach (var cooldown in _cooldowns)
        {
            cooldown.Current -= Time.deltaTime;
            if (cooldown.Current <= 0f)
            {
                cooldown.Current = 0f;
                _cooldownCompleteQueue.Enqueue(cooldown);
            }
        }

        while (_cooldownCompleteQueue.Count > 0)
        {
            _cooldowns.Remove(_cooldownCompleteQueue.Peek());
            _cooldownCompleteQueue.Dequeue();
        }
    }

    public void AddCooldown(Cooldown cooldown)
    {
        _cooldowns.Add(cooldown);
    }

    public void Clear()
    {
        foreach (var itemData in CooldownableDatabase.GetInstance.CooldownableItems)
        {
            (itemData as ICooldownable).Cooldown.Clear();
        }

        foreach (var skillData in CooldownableDatabase.GetInstance.CooldownableSkills)
        {
            skillData.Cooldown.Clear();
        }

        _cooldowns.Clear();
        _cooldownCompleteQueue.Clear();
    }
}
