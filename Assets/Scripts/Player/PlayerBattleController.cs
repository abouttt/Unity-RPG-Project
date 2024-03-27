using UnityEngine;

public class PlayerBattleController : MonoBehaviour
{
    public bool CanAttack { get; set; } = true;
    public bool CanParry { get; set; } = true;
    public bool CanDefense { get; set; } = true;

    public bool IsAttacking { get; private set; } = false;
    public bool IsParrying { get; private set; } = false;
    public bool IsDefending { get; private set; } = false;
    public bool IsDefenseDamaging { get; private set; } = false;
    public bool IsDamaging { get; private set; } = false;

    public bool Enabled
    {
        get => _enabled;
        set
        {
            _enabled = value;
            CanAttack = value;
            CanParry = value;
            CanDefense = value;
        }
    }

    [SerializeField]
    private float _defenseAngle;

    [SerializeField]
    private float _attackRequiredSP;

    [SerializeField]
    private float _parryRequiredSP;

    [SerializeField]
    private float _defenseDamagedRequiredSP;

    private readonly int _animIDAttack = Animator.StringToHash("Attack");
    private readonly int _animIDParry = Animator.StringToHash("Parry");
    private readonly int _animIDDefense = Animator.StringToHash("Defense");
    private readonly int _animIDDamaged = Animator.StringToHash("Damaged");

    private bool _hasReservedAttack = false;
    private bool _isParryable = false;
    private bool _enabled = true;

    private Weapon _weapon;

    private void Awake()
    {
        Player.Root.EquipmentChanged += (type) =>
        {
            if (type == EquipmentType.Weapon)
            {
                if (Player.EquipmentInventory.IsEquipped(EquipmentType.Weapon))
                {
                    _weapon = Player.Root.GetEquipment(EquipmentType.Weapon).GetComponent<Weapon>();
                }
                else
                {
                    _weapon = null;
                }
            }
        };
    }

    private void Update()
    {
        Player.Animator.SetBool(_animIDAttack, false);
        Player.Animator.SetBool(_animIDParry, false);

        if (!Managers.Input.CursorLocked || Player.Movement.IsJumping || Player.Movement.IsRolling)
        {
            if (IsDefending)
            {
                OffDefense();
            }

            return;
        }

        if (Managers.Input.Attack && Player.EquipmentInventory.IsEquipped(EquipmentType.Weapon))
        {
            _hasReservedAttack = true;
        }

        if (_hasReservedAttack)
        {
            Attack();
            return;
        }

        if (Managers.Input.Parry && Player.EquipmentInventory.IsEquipped(EquipmentType.Shield))
        {
            Parry();
            return;
        }

        if (Managers.Input.Defense && Player.EquipmentInventory.IsEquipped(EquipmentType.Shield))
        {
            Defense();
        }
        else if (IsDefending)
        {
            OffDefense();
        }
    }

    public void TakeDamage(Monster monster, Vector3 attackedPosition, int damage, bool parryable)
    {
        if (Player.Status.HP <= 0)
        {
            return;
        }

        if (Player.Movement.IsRolling)
        {
            return;
        }

        if (parryable && _isParryable)
        {
            if (IsInRangeOfDefenseAngle(attackedPosition))
            {
                monster.Stunned();
                Managers.Resource.Instantiate(
                    "ParryHit.prefab", Player.Root.GetEquipment(EquipmentType.Shield).transform.position, null, true);
                return;
            }
        }
        else if (IsDefending)
        {
            if (IsInRangeOfDefenseAngle(attackedPosition))
            {
                HitShield();
                return;
            }
            else
            {
                OffDefense();
                CanDefense = false;
            }
        }

        IsDamaging = true;
        Player.Status.HP -= Mathf.Clamp(Util.CalcDamage(damage, Player.Status.Defense), 0, damage);

        if (Player.Status.HP <= 0)
        {
            Player.Animator.Play("Dead", -1, 0f);
        }
        else
        {
            Player.Animator.Play("Damaged", -1, 0f);
            Player.Animator.SetBool(_animIDDamaged, true);
        }
    }

    public void HitShield(Vector3? hitPosition = null)
    {
        if (!IsDefending)
        {
            return;
        }

        IsDefenseDamaging = true;
        Player.Animator.Play("DefenseDamaged", -1, 0f);
        Player.Status.SP -= _defenseDamagedRequiredSP;
        var pos = hitPosition != null ? hitPosition.Value : Player.Root.GetEquipment(EquipmentType.Shield).transform.position;
        Managers.Resource.Instantiate("SteelHit.prefab", pos, null, true);
    }

    public void Clear()
    {
        IsAttacking = false;
        IsParrying = false;
        IsDefending = false;
        IsDefenseDamaging = false;
        IsDamaging = false;
        _hasReservedAttack = false;
        _isParryable = false;
        Player.Animator.SetBool(_animIDDamaged, false);
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
        Player.Status.SP -= _attackRequiredSP;
        Player.Animator.SetBool(_animIDAttack, true);
    }

    private void Parry()
    {
        if (!CanParry)
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

        IsParrying = true;
        Player.Movement.CanRotation = true;
        Player.Status.SP -= _parryRequiredSP;
        Player.Animator.SetBool(_animIDParry, true);
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

    private bool IsInRangeOfDefenseAngle(Vector3 attackedPosition)
    {
        var dir = (attackedPosition - transform.position).normalized;
        if (Vector3.Angle(transform.forward, dir) < _defenseAngle * 0.5f)
        {
            return true;
        }

        return false;
    }

    private void OnCanAttackCombo()
    {
        CanAttack = true;
        OnCanRoll();
    }

    private void OnEnableWeapon()
    {
        _weapon.Collider.enabled = true;
    }

    private void OnDisableWeapon()
    {
        _weapon.Collider.enabled = false;
    }

    private void OnEnableParry()
    {
        _isParryable = true;
    }

    private void OnDisableParry()
    {
        _isParryable = false;
    }

    private void OnCanRoll()
    {
        Player.Movement.CanRoll = true;
    }
}
