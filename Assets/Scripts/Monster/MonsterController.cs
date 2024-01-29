using System;
using UnityEngine;

public class MonsterController : MonoBehaviour
{
    public event Action HPChanged;

    [field: SerializeField]
    public MonsterStat Stat { get; private set; }

    public int CurrentHP { get; set; }
    public int CurrentDamage { get; private set; }

    public bool IsLockOnTarget
    {
        get => _isLockOnTarget;
        set
        {
            _isLockOnTarget = value;
            if (_isLockOnTarget)
            {
                ShowHPBar();
            }
        }
    }

    private UI_MonsterHPBar _hpBar;
    private bool _isLockOnTarget;

    private void OnEnable()
    {
        CurrentHP = Stat.MaxHP;
    }

    private void ShowHPBar()
    {
        if (_hpBar == null)
        {
            _hpBar = Managers.Resource.Instantiate("UI_MonsterHPBar",
                Managers.UI.Get<UI_AutoCanvas>().transform, true).GetComponent<UI_MonsterHPBar>();
            _hpBar.SetTarget(this);
        }
        else
        {
            HPChanged?.Invoke();
        }
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
