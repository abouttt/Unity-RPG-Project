using UnityEngine;

public class PlayerBattleController : MonoBehaviour
{
    public bool IsAttacking { get; private set; } = false;
    public bool IsParrying { get; private set; } = false;
    public bool IsDefending { get; private set; } = false;
    public bool IsDamaging { get; private set; } = false;
    public bool CanAttack { get; set; } = true;
    public bool CanParry { get; set; } = true;
    public bool CanDefense { get; set; } = true;

    [SerializeField]
    private float _defenseAngle;

    [SerializeField]
    private float _requiredAttackSP;

    [SerializeField]
    private float _requiredParrySP;

    [SerializeField]
    private float _requiredDefenseSP;

    private readonly Collider[] _monsters = new Collider[10];

    private readonly int _animIDAttack = Animator.StringToHash("Attack");
    private readonly int _animIDParry = Animator.StringToHash("Parry");
    private readonly int _animIDDefense = Animator.StringToHash("Defense");
    private readonly int _animIDDamaged = Animator.StringToHash("Damaged");

    private int _currentAttackComboCount;
    private bool _hasReservedAttack = false;
    private bool _isParryable = false;

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
            }
        };
    }

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

        if (Managers.Input.Attack && Player.EquipmentInventory.IsEquipped(EquipmentType.Weapon))
        {
            _hasReservedAttack = true;
        }

        if (_hasReservedAttack)
        {
            Attack();
            return;
        }

        if (Player.EquipmentInventory.IsEquipped(EquipmentType.Shield))
        {
            if (Managers.Input.Defense)
            {
                Defense();
            }
            else if (IsDefending)
            {
                OffDefense();
            }

            if (Managers.Input.Parry)
            {
                Parry();
            }
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

        if (parry && _isParryable)
        {
            if (IsRangeOfDefenseAngle(attackedPosition))
            {
                monster.Stunned();
                Managers.Resource.Instantiate(
                    "ShieldHit.prefab", Player.Root.GetEquipment(EquipmentType.Shield).transform.position, null, true);
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
            Player.Animator.SetTrigger(_animIDDamaged);
        }
    }

    public void HitShield(Vector3? hitPosition = null)
    {
        if (!IsDefending)
        {
            return;
        }

        Player.Animator.Play("DefenseDamaged", -1, 0f);
        Player.Status.SP -= _requiredDefenseSP;
        var pos = hitPosition != null ? hitPosition.Value : Player.Root.GetEquipment(EquipmentType.Shield).transform.position;
        Managers.Resource.Instantiate("ShieldHit.prefab", pos, null, true);
    }

    public void Clear()
    {
        IsAttacking = false;
        IsParrying = false;
        IsDamaging = false;
        _currentAttackComboCount = 0;
        _hasReservedAttack = false;
        _isParryable = false;
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
        Player.Status.SP -= _requiredAttackSP;
        Player.Animator.SetTrigger(_animIDAttack);
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
        Player.Status.SP -= _requiredParrySP;
        Player.Animator.SetTrigger(_animIDParry);
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
            if (_monsters[i].GetComponent<Monster>().TakeDamage(damage))
            {
                //Managers.Resource.Instantiate("SwordHit.prefab", _monsters[i].bounds.center, null, true);
            }
        }

        return monsterCnt > 0;
    }

    private bool IsRangeOfDefenseAngle(Vector3 attackedPosition)
    {
        var dir = (attackedPosition - transform.position).normalized;
        if (Vector3.Angle(transform.forward, dir) < _defenseAngle)
        {
            return true;
        }

        return false;
    }

    private void OnEnableWeapon()
    {
        //CreateAttackEffect();
        _weapon.Collider.enabled = true;
        //GiveDamageToMonster(_attackOffset.position,
        //Player.EquipmentInventory.GetItem(EquipmentType.Weapon).EquipmentData.AttackRadius, Player.Status.Damage);
    }

    private void OnDisableWeapon()
    {
        _weapon.Collider.enabled = false;
    }

    private void OnCanAttackCombo()
    {
        if (IsAttacking)
        {
            int nextCount = _currentAttackComboCount + 1;
            _currentAttackComboCount = nextCount >= 4 ? 0 : nextCount;
        }

        CanAttack = true;
        OnCanRoll();
    }

    private void OnCanRoll()
    {
        Player.Movement.CanRoll = true;
    }

    private void OnBeginParry()
    {
        _isParryable = true;
    }

    private void OnEndParry()
    {
        _isParryable = false;
    }
}
