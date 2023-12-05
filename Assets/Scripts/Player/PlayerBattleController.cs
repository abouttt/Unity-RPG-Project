using System.Collections.Generic;
using UnityEngine;

public class PlayerBattleController : MonoBehaviour
{
    public bool IsAttacking { get; private set; } = false;
    public bool IsDefending { get; private set; } = false;
    public bool CanAttack { get; set; } = true;

    [SerializeField]
    private Transform _attackOffset;
    [SerializeField]
    private float _requiredAttackSP;
    [SerializeField]
    private List<Vector3> _attackEffectDirection;
    private int _currentAttackComboCount = 0;

    private bool _hasReservedAttack = false;

    private readonly int _animIDAttack = Animator.StringToHash("Attack");

    private void Update()
    {
        if (!Managers.Input.CursorLocked || Player.Movement.IsJumping)
        {
            return;
        }

        if (Player.Root.IsEquip(EquipmentType.Weapon) && Managers.Input.Attack)
        {
            _hasReservedAttack = true;
        }

        if (_hasReservedAttack)
        {
            Attack();
        }
    }

    private void Attack()
    {
        if (!CanMelee())
        {
            return;
        }

        if (Player.Status.SP < _requiredAttackSP * 0.5f)
        {
            return;
        }

        _hasReservedAttack = false;
        IsAttacking = true;
        Player.Movement.CanRotation = true;
        Player.Animator.SetTrigger(_animIDAttack);
        Player.Status.SP -= _requiredAttackSP;
    }

    private bool CanMelee()
    {
        return CanAttack && Player.Movement.IsGrounded;
    }

    private void OnBeginMeleeAnim()
    {
        CanAttack = false;
        Player.Movement.CanMove = false;
        Player.Movement.CanRotation = false;
        Player.Movement.CanJump = false;
    }

    private void OnCanAttackCombo()
    {
        if (IsAttacking)
        {
            _currentAttackComboCount = (_currentAttackComboCount + 1) >= _attackEffectDirection.Count ? 0 : _currentAttackComboCount + 1;
        }

        CanAttack = true;
    }

    private void OnEndMeleeAnim()
    {
        _hasReservedAttack = false;
        _currentAttackComboCount = 0;
        IsAttacking = false;
        CanAttack = true;
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
