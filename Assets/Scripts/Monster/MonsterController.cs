using System;
using UnityEngine;

public class MonsterController : MonoBehaviour
{
    public event Action HPChanged;

    [field: SerializeField]
    public MonsterStat MaxStat { get; private set; }

    public int CurrentHP { get; set; }

    public bool IsLockOnTarget { get; set; }

    private void OnEnable()
    {
        CurrentHP = MaxStat.MaxHP;
    }

    private void OnIdle()
    {

    }

    private void OnTracking()
    {

    }

    private void OnAttack()
    {

    }

    private void OnDamaged()
    {

    }

    private void OnRestorePosition()
    {

    }

    private void OnDead()
    {

    }
}
