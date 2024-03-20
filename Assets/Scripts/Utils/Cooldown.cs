using System;
using UnityEngine;

[Serializable]
public class Cooldown
{
    [field: SerializeField]
    public float Max { get; private set; }
    public float Current;
    public event Action Cooldowned;

    public void Clear()
    {
        Current = 0;
        Cooldowned = null;
    }

    public void OnCooldowned()
    {
        Current = Max;
        Managers.Cooldown.AddCooldown(this);
        Cooldowned?.Invoke();
    }
}
