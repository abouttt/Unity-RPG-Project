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

    public virtual void OnCooldowned()
    {
        Current = Max;
        Cooldowned?.Invoke();
        Managers.Cooldown.AddCooldown(this);
    }
}
