using System;
using UnityEngine;

public class Monster : MonoBehaviour
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

    private void Awake()
    {
        gameObject.layer = LayerMask.NameToLayer("Monster");
    }

    private void OnEnable()
    {
        CurrentHP = Stat.MaxHP;
    }

    public void TakeDamage(int damage)
    {
        ShowHPBar();
        CurrentDamage = Mathf.Clamp(damage - Stat.Defense, 0, damage);
        CurrentHP -= CurrentDamage;
        HPChanged?.Invoke();
        CurrentDamage = 0;

        if (CurrentHP <= 0)
        {
            // DIE
        }
        else
        {
            // DAMAGED
        }
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
}
