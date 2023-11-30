using System.Collections.Generic;
using UnityEngine;

public class CooldownManager
{
    private readonly HashSet<Cooldown> _cooldowns = new();
    private readonly Queue<Cooldown> _cooldownCompleteQueue = new();

    public void LateUpdate()
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

    public void AddCooldown(Cooldown Cooldownable)
    {
        _cooldowns.Add(Cooldownable);
    }

    public void Clear()
    {
        _cooldowns.Clear();
        _cooldownCompleteQueue.Clear();
    }
}
