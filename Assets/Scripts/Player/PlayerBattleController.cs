using System.Collections.Generic;
using UnityEngine;

public class PlayerBattleController : MonoBehaviour
{
    public bool IsAttacking { get; private set; } = false;
    public bool IsDefending { get; private set; } = false;
    public bool CanAttack { get; set; } = true;
    public bool CanDefense { get; set; } = true;

    [SerializeField]
    private Transform _attackOffset;
    [SerializeField]
    private float _requiredAttackSP;
    [SerializeField]
    private List<Vector3> _attackEffectDirection;
    private int _currentAttackComboCount = 0;

    private bool _hasReservedAttack = false;

    private readonly int _animIDAttack = Animator.StringToHash("Attack");
    private readonly int _animIDDefense = Animator.StringToHash("Defense");

    private void Update()
    {
        if (!Managers.Input.CursorLocked || Player.Movement.IsJumping)
        {
            if (IsDefending)
            {
                OffDefense();
            }

            return;
        }

        if (Player.Root.IsEquip(EquipmentType.Weapon) && Managers.Input.Attack)
        {
            _hasReservedAttack = true;
        }

        if (_hasReservedAttack)
        {
            Attack();
            return;
        }

        if (Player.Root.IsEquip(EquipmentType.Shield) && Managers.Input.Defense)
        {
            Defense();
        }
        else if (IsDefending)
        {
            OffDefense();
        }
    }

    private void Attack()
    {
        if (!CanAttack)
        {
            return;
        }

        _hasReservedAttack = false;

        if (!Player.Movement.IsGrounded)
        {
            return;
        }

        if (Player.Status.SP < _requiredAttackSP * 0.5f)
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

    private void Defense()
    {
        if (!CanDefense && Player.Movement.IsGrounded)
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

    private void OnBeginMeleeAnim()
    {
        CanAttack = false;
        CanDefense = false;
        Player.Movement.CanMove = false;
        Player.Movement.CanRotation = false;
        Player.Movement.CanJump = false;
    }

    private void OnCanAttackCombo()
    {
        if (IsAttacking)
        {
            int nextCount = _currentAttackComboCount + 1;
            _currentAttackComboCount = nextCount >= _attackEffectDirection.Count ? 0 : nextCount;
        }

        CanAttack = true;
    }

    private void OnEndMeleeAnim()
    {
        _currentAttackComboCount = 0;
        _hasReservedAttack = false;
        IsAttacking = false;
        CanAttack = true;
        CanDefense = true;
        Player.Movement.CanMove = true;
        Player.Movement.CanRotation = true;
        Player.Movement.CanJump = true;
        Player.Animator.ResetTrigger(_animIDAttack);
    }

    private void OnEnableWeapon()
    {
        CreateEffect();
    }

    private void CreateEffect()
    {
        var euler = transform.rotation.eulerAngles;
        euler.y += 90;
        euler += _attackEffectDirection[_currentAttackComboCount];
        Managers.Resource.Instantiate("AttackSlash",
            Player.Root.GetRoot(EquipmentType.Weapon).transform.position, Quaternion.Euler(euler), null, true);
    }
}
