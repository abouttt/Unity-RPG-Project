using System;
using UnityEngine;

public class PlayerStatus : MonoBehaviour
{
    public event Action LevelChanged;
    public event Action HPChanged;
    public event Action MPChanged;
    public event Action SPChanged;
    public event Action XPChanged;
    public event Action StatChanged;
    public event Action GoldChanged;
    public event Action SkillPointChanged;

    public int Level { get; private set; } = 1;

    public int HP
    {
        get => _currentStat.HP;
        set
        {
            _currentStat.HP = Mathf.Clamp(value, 0, MaxStat.HP);
            HPChanged?.Invoke();
        }
    }

    public int MP
    {
        get => _currentStat.MP;
        set
        {
            _currentStat.MP = Mathf.Clamp(value, 0, MaxStat.MP);
            MPChanged?.Invoke();
        }
    }

    public float SP
    {
        get => _currentStat.SP;
        set
        {
            var prevSP = _currentStat.SP;
            _currentStat.SP = Mathf.Clamp(value, 0, MaxStat.SP);
            if (_currentStat.SP < prevSP)
            {
                _recoverySPTime = 0f;
            }

            SPChanged?.Invoke();
        }
    }

    public int XP
    {
        get => _currentStat.XP;
        set
        {
            if (IsMaxLevel || MaxStat.XP == 0)
            {
                return;
            }

            _currentStat.XP = value;

            while(_currentStat.XP >= MaxStat.XP)
            {
                _currentStat.XP -= MaxStat.XP;
                LevelUp();
            }

            XPChanged?.Invoke();
        }
    }

    public int Damage => _currentStat.Damage;
    public int Defense => _currentStat.Defense;

    public int Gold
    {
        get => _currentGold;
        set
        {
            _currentGold = value;
            GoldChanged?.Invoke();
        }
    }

    public int SkillPoint
    {
        get => _skillPoint;
        set
        {
            _skillPoint = value;
            SkillPointChanged?.Invoke();
        }
    }

    [ReadOnly]
    public PlayerStat ExtraStat = new();
    public PlayerStat MaxStat { get; private set; } = new();
    public bool IsMaxLevel => Level >= _playerStatTable.StatTable.Count;

    [SerializeField]
    private float _recoverySPDelay;
    [SerializeField]
    private float _recoverySPAmount;
    [SerializeField]
    private PlayerStatTable _playerStatTable;

    private float _recoverySPTime;  // SP 회복 현재 딜레이 시간

    private readonly PlayerStat _currentStat = new();
    private int _currentGold = 0;
    private int _skillPoint = 0;

    private void Awake()
    {
        RefreshAllStat();
        FillAllStat();
        StatChanged?.Invoke();
    }

    private void Start()
    {
        Player.EquipmentInventory.EquipmentChanged += (equipmentType) =>
        {
            RefreshAllStat();
            FillCurrentMeleeStat();
            if (_currentStat.HP > MaxStat.HP)
            {
                _currentStat.HP = MaxStat.HP;
            }
            if (_currentStat.MP > MaxStat.MP)
            {
                _currentStat.MP = MaxStat.MP;
            }
            if (_currentStat.SP > MaxStat.SP)
            {
                _currentStat.SP = MaxStat.SP;
            }
            StatChanged?.Invoke();
        };
    }

    private void Update()
    {
        if (!Player.Movement.CanSprint && !Managers.Input.Sprint)
        {
            Player.Movement.CanSprint = true;
        }

        RecoverySP();
    }

    public void RefreshAllStat()
    {
        int level = (IsMaxLevel ? _playerStatTable.StatTable.Count : Level) - 1;

        MaxStat.HP = _playerStatTable.StatTable[level].HP + ExtraStat.HP;
        MaxStat.MP = _playerStatTable.StatTable[level].MP + ExtraStat.MP;
        MaxStat.SP = _playerStatTable.StatTable[level].SP + ExtraStat.SP;
        MaxStat.XP = _playerStatTable.StatTable[level].XP;
        MaxStat.Damage = _playerStatTable.StatTable[level].Damage + ExtraStat.Damage;
        MaxStat.Defense = _playerStatTable.StatTable[level].Defense + ExtraStat.Defense;

        var equipmentTypes = Enum.GetValues(typeof(EquipmentType));
        foreach (EquipmentType equipmentType in equipmentTypes)
        {
            AddStatByEquipment(equipmentType);
        }
    }

    private void AddStatByEquipment(EquipmentType equipmentType)
    {
        if (Player.EquipmentInventory.IsNullSlot(equipmentType))
        {
            return;
        }

        MaxStat.HP += Player.EquipmentInventory.GetItem(equipmentType).EquipmentData.HP;
        MaxStat.MP += Player.EquipmentInventory.GetItem(equipmentType).EquipmentData.MP;
        MaxStat.SP += Player.EquipmentInventory.GetItem(equipmentType).EquipmentData.SP;
        MaxStat.Damage += Player.EquipmentInventory.GetItem(equipmentType).EquipmentData.Damage;
        MaxStat.Defense += Player.EquipmentInventory.GetItem(equipmentType).EquipmentData.Defense;
    }

    private void LevelUp()
    {
        Level++;
        SkillPoint++;
        RefreshAllStat();
        FillAllStat();
        LevelChanged?.Invoke();
        StatChanged?.Invoke();
    }

    private void FillAllStat()
    {
        FillCurrentStat();
        FillCurrentMeleeStat();
    }

    private void FillCurrentStat()
    {
        _currentStat.HP = MaxStat.HP;
        _currentStat.MP = MaxStat.MP;
        _currentStat.SP = MaxStat.SP;
    }

    private void FillCurrentMeleeStat()
    {
        _currentStat.Damage = MaxStat.Damage;
        _currentStat.Defense = MaxStat.Defense;
    }

    // SP 딜레이 시간이 넘으면 SP회복
    private void RecoverySP()
    {
        if (!Player.Movement.IsGrounded)
        {
            _recoverySPTime = 0;
            return;
        }

        _recoverySPTime += Time.deltaTime;

        if (_recoverySPTime >= _recoverySPDelay)
        {
            if (SP < MaxStat.SP)
            {
                SP += Mathf.Clamp(_recoverySPAmount * Time.deltaTime, 0f, MaxStat.SP);
            }
        }
    }
}
