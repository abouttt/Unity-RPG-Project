using System.Collections.Generic;
using UnityEngine;

public class PlayerBattleController : MonoBehaviour
{
    public bool IsAttacking { get; private set; } = false;
    public bool IsDefending { get; private set; } = false;
    public bool IsDamaging { get; private set; } = false;
    public bool CanAttack { get; set; } = true;
    public bool CanDefense { get; set; } = true;

    [SerializeField]
    private Transform _attackOffset;
    [SerializeField]
    private float _defenseAngle;
    [SerializeField]
    private float _requiredAttackSP;
    [SerializeField]
    private float _requiredParrySP;
    [SerializeField]
    private float _requiredDefenseSP;
    [SerializeField]
    private List<Vector3> _attackEffectDirection;

    private int _currentAttackComboCount = 0;
    private bool _hasReservedAttack = false;
    private bool _canParry = false;

    private readonly Collider[] _monsters = new Collider[10];

    private readonly int _animIDAttack = Animator.StringToHash("Attack");
    private readonly int _animIDParry = Animator.StringToHash("Parry");
    private readonly int _animIDDefense = Animator.StringToHash("Defense");
    private readonly int _animIDDefenseDamaged = Animator.StringToHash("DefenseDamaged");

    private void Update()
    {
        if (!Managers.Input.CursorLocked || Player.Movement.IsJumping || Player.Movement.IsRolling)
        {
            if (IsDefending)
            {
                OffDefense();
            }

            return;
        }

        if (Managers.Input.Attack && Player.Root.IsEquip(EquipmentType.Weapon))
        {
            _hasReservedAttack = true;
        }

        if (_hasReservedAttack)
        {
            Attack();
            return;
        }

        if (Managers.Input.Defense && Player.Root.IsEquip(EquipmentType.Shield))
        {
            Defense();
        }
        else if (IsDefending)
        {
            OffDefense();
        }

        if (Managers.Input.Parry && Player.Root.IsEquip(EquipmentType.Shield))
        {
            Parry();
        }
    }

    public void TakeDamage(Monster monster, Vector3 attackedPosition, int damage, bool parry)
    {
        if (Player.Status.HP <= 0)
        {
            return;
        }

        if (Player.Movement.IsRolling)
        {
            return;
        }

        if (_canParry)
        {
            if (IsRangeOfDefenseAngle(attackedPosition))
            {
                monster.Stunned();
                return;
            }
        }
        else if (IsDefending)
        {
            if (IsRangeOfDefenseAngle(attackedPosition))
            {
                HitShield();
                return;
            }
            else
            {
                IsDefending = false;
                CanDefense = false;
                Player.Animator.SetBool(_animIDDefense, false);
            }
        }

        IsDamaging = true;
        Player.Status.HP -= Mathf.Clamp(damage - Player.Status.Defense, 0, damage);

        if (Player.Status.HP <= 0)
        {
            Player.Animator.Play("Dead", -1, 0f);
        }
        else
        {
            Player.Animator.Play("Damaged", -1, 0f);
        }
    }

    public void HitShield()
    {
        if (!IsDefending)
        {
            return;
        }

        Player.Animator.SetTrigger(_animIDDefenseDamaged);
        Player.Status.SP -= _requiredDefenseSP;
    }

    public void ClearBattleInfo()
    {
        _currentAttackComboCount = 0;
        _hasReservedAttack = false;
        _canParry = false;
        IsAttacking = false;
        IsDamaging = false;
        Player.Animator.ResetTrigger(_animIDAttack);
        Player.Animator.ResetTrigger(_animIDParry);
    }

    private void Attack()
    {
        if (!CanAttack)
        {
            return;
        }

        _hasReservedAttack = false;

        if (Player.Status.SP <= 0f)
        {
            return;
        }

        if (IsDefending)
        {
            OffDefense();
            CanDefense = false;
        }

        IsAttacking = true;
        Player.Movement.CanRotation = true;
        Player.Animator.SetTrigger(_animIDAttack);
        Player.Status.SP -= _requiredAttackSP;
    }

    private void Parry()
    {
        if (!CanAttack)
        {
            return;
        }

        if (Player.Status.SP <= 0f)
        {
            return;
        }

        if (IsDefending)
        {
            OffDefense();
            CanDefense = false;
        }

        Player.Animator.SetTrigger(_animIDParry);
        Player.Status.SP -= _requiredParrySP;
    }

    private void Defense()
    {
        if (!CanDefense)
        {
            return;
        }

        IsDefending = true;
        Player.Animator.SetBool(_animIDDefense, true);
    }

    private void OffDefense()
    {
        IsDefending = false;
        Player.Animator.SetBool(_animIDDefense, false);
    }

    private bool GiveDamageToMonster(Vector3 attackOffset, float radius, int damage)
    {
        int monsterCnt = Physics.OverlapSphereNonAlloc(attackOffset, radius, _monsters, 1 << LayerMask.NameToLayer("Monster"));

        for (int i = 0; i < monsterCnt; i++)
        {
            _monsters[i].GetComponent<Monster>().TakeDamage(damage);
            Managers.Resource.Instantiate("SwordHitMiniYellow", _monsters[i].bounds.center, null, true);
        }

        return monsterCnt > 0;
    }

    private bool IsRangeOfDefenseAngle(Vector3 attackedPosition)
    {
        Vector3 dir = (attackedPosition - transform.position).normalized;
        if (Vector3.Angle(transform.forward, dir) < _defenseAngle)
        {
            return true;
        }

        return false;
    }

    private void OnCanAttackCombo()
    {
        if (IsAttacking)
        {
            int nextCount = _currentAttackComboCount + 1;
            _currentAttackComboCount = nextCount >= _attackEffectDirection.Count ? 0 : nextCount;
        }

        CanAttack = true;
        Player.Movement.CanRoll = true;
    }

    private void OnEnableWeapon()
    {
        CreateAttackEffect();
        GiveDamageToMonster(_attackOffset.transform.position,
            Player.EquipmentInventory.GetItem(EquipmentType.Weapon).EquipmentData.AttackRadius, Player.Status.Damage);
    }

    private void OnBeginParry()
    {
        _canParry = true;
    }

    private void OnEndParry()
    {
        _canParry = false;
    }

    private void CreateAttackEffect()
    {
        var euler = transform.rotation.eulerAngles;
        euler.y += 90f;
        euler += _attackEffectDirection[_currentAttackComboCount];
        Managers.Resource.Instantiate("AttackSlash",
            Player.Root.GetRoot(EquipmentType.Weapon).transform.position, Quaternion.Euler(euler), null, true);
    }
}
