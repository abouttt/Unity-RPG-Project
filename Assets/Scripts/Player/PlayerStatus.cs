using System;
using UnityEngine;
using Newtonsoft.Json.Linq;

public class PlayerStatus : MonoBehaviour, ISavable
{
    public static string SaveKey => "SaveStatus";

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
            _currentStat.HP = Mathf.Clamp(value, 0, _maxStat.HP);
            HPChanged?.Invoke();
        }
    }

    public int MP
    {
        get => _currentStat.MP;
        set
        {
            _currentStat.MP = Mathf.Clamp(value, 0, _maxStat.MP);
            MPChanged?.Invoke();
        }
    }

    public float SP
    {
        get => _currentStat.SP;
        set
        {
            float prevSP = _currentStat.SP;
            _currentStat.SP = Mathf.Clamp(value, 0f, _maxStat.SP);
            if (_currentStat.SP < prevSP)
            {
                _recoverySPDeltaTime = 0f;
            }

            SPChanged?.Invoke();
        }
    }

    public int XP
    {
        get => _currentStat.XP;
        set
        {
            if (IsMaxLevel || _maxStat.XP == 0)
            {
                return;
            }

            _currentStat.XP = value;

            int level = 0;
            while (_currentStat.XP >= _maxStat.XP)
            {
                _currentStat.XP -= _maxStat.XP;
                level++;
            }

            if (level > 0)
            {
                LevelUp(level);
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

    public bool IsMaxLevel => Level >= _playerStatTable.StatTable.Count;

    public int MaxHP => _maxStat.HP;
    public int MaxMP => _maxStat.MP;
    public int MaxSP => (int)_maxStat.SP;
    public int MaxXP => _maxStat.XP;
    public int MaxDamage => _maxStat.Damage;
    public int MaxDefense => _maxStat.Defense;

    public PlayerStatData ExtraFixedStat
    {
        get => _extraFixedStat;
        set
        {
            _extraFixedStat = value;
            RefreshAllStat();
            StatChanged?.Invoke();
        }
    }

    public PlayerStatData ExtraPerStat
    {
        get => _extraPerStat;
        set
        {
            _extraPerStat = value;
            RefreshAllStat();
            StatChanged?.Invoke();
        }
    }

    [SerializeField]
    private PlayerStatTable _playerStatTable;

    [SerializeField]
    private float _recoverySPDelay;

    [SerializeField]
    private float _recoverySPAmount;

    private readonly PlayerStatData _maxStat = new();
    private readonly PlayerStatData _currentStat = new();
    private PlayerStatData _extraFixedStat = new();
    private PlayerStatData _extraPerStat = new();

    private int _currentGold;
    private int _skillPoint;
    private float _recoverySPDeltaTime;  // SP 회복 현재 딜레이 시간

    private void Awake()
    {
        LoadSaveData();
        RefreshAllStat();
        FillCurrentMeleeStat();
        if (Managers.Data.HasSaveData)
        {
            SP = _maxStat.SP;
        }
        else
        {
            FillCurrentAbilityStat();
        }
    }

    private void Start()
    {
        Player.EquipmentInventory.InventoryChanged += equipmentType =>
        {
            RefreshAllStat();
            FillCurrentMeleeStat();
            if (_currentStat.HP > _maxStat.HP)
            {
                _currentStat.HP = _maxStat.HP;
            }
            if (_currentStat.MP > _maxStat.MP)
            {
                _currentStat.MP = _maxStat.MP;
            }
            if (_currentStat.SP > _maxStat.SP)
            {
                _currentStat.SP = _maxStat.SP;
            }
            StatChanged?.Invoke();
        };

        StatChanged?.Invoke();
    }

    private void Update()
    {
        RecoverySP();
    }

    public void RefreshAllStat()
    {
        int level = (IsMaxLevel ? _playerStatTable.StatTable.Count : Level) - 1;
        _maxStat.HP = _playerStatTable.StatTable[level].HP + ExtraFixedStat.HP;
        _maxStat.MP = _playerStatTable.StatTable[level].MP + ExtraFixedStat.MP;
        _maxStat.SP = _playerStatTable.StatTable[level].SP + ExtraFixedStat.SP;
        _maxStat.XP = _playerStatTable.StatTable[level].XP;
        _maxStat.Damage = _playerStatTable.StatTable[level].Damage + ExtraFixedStat.Damage;
        _maxStat.Defense = _playerStatTable.StatTable[level].Defense + ExtraFixedStat.Defense;

        var types = Enum.GetValues(typeof(EquipmentType));
        foreach (EquipmentType equipmentType in types)
        {
            var equipment = Player.EquipmentInventory.GetItem(equipmentType);
            if (equipment == null)
            {
                continue;
            }

            _maxStat.HP += equipment.EquipmentData.HP;
            _maxStat.MP += equipment.EquipmentData.MP;
            _maxStat.SP += equipment.EquipmentData.SP;
            _maxStat.Damage += equipment.EquipmentData.Damage;
            _maxStat.Defense += equipment.EquipmentData.Defense;
        }

        _maxStat.HP = Util.CalcIncreasePer(_maxStat.HP, ExtraPerStat.HP);
        _maxStat.MP = Util.CalcIncreasePer(_maxStat.MP, ExtraPerStat.MP);
        _maxStat.SP = Util.CalcIncreasePer((int)_maxStat.SP, (int)ExtraPerStat.SP);
        _maxStat.Damage = _currentStat.Damage = Util.CalcIncreasePer(_maxStat.Damage, ExtraPerStat.Damage);
        _maxStat.Defense = _currentStat.Defense = Util.CalcIncreasePer(_maxStat.Defense, ExtraPerStat.Defense);
    }

    public void FillAllStat()
    {
        FillCurrentAbilityStat();
        FillCurrentMeleeStat();
    }

    public void FillCurrentAbilityStat()
    {
        _currentStat.HP = _maxStat.HP;
        _currentStat.MP = _maxStat.MP;
        _currentStat.SP = _maxStat.SP;
    }

    public void FillCurrentMeleeStat()
    {
        _currentStat.Damage = _maxStat.Damage;
        _currentStat.Defense = _maxStat.Defense;
    }

    public JArray CreateSaveData()
    {
        var saveData = new JArray();

        var statusSaveData = new StatusSaveData()
        {
            Level = Level,
            CurrentHP = _currentStat.HP,
            CurrentMP = _currentStat.MP,
            CurrentXP = _currentStat.XP,
            Gold = Gold,
            SkillPoint = SkillPoint,
        };

        saveData.Add(JObject.FromObject(statusSaveData));

        return saveData;
    }

    public void LoadSaveData()
    {
        if (!Managers.Data.Load<JArray>(SaveKey, out var saveData))
        {
            return;
        }

        var statusSaveData = saveData[0].ToObject<StatusSaveData>();

        Level = statusSaveData.Level;
        Gold = statusSaveData.Gold;
        SkillPoint = statusSaveData.SkillPoint;
        _currentStat.HP = statusSaveData.CurrentHP;
        _currentStat.MP = statusSaveData.CurrentMP;
        _currentStat.XP = statusSaveData.CurrentXP;
    }

    private void LevelUp(int level)
    {
        if (level <= 0)
        {
            return;
        }

        Level += level;
        RefreshAllStat();
        FillAllStat();
        LevelChanged?.Invoke();
        StatChanged?.Invoke();
    }

    // SP 딜레이 시간이 넘으면 SP회복
    private void RecoverySP()
    {
        if (!Player.Movement.IsGrounded)
        {
            _recoverySPDeltaTime = 0f;
            return;
        }

        _recoverySPDeltaTime += Time.deltaTime;
        if (_recoverySPDeltaTime >= _recoverySPDelay)
        {
            if (SP < _maxStat.SP)
            {
                SP += Mathf.Clamp(_recoverySPAmount * Time.deltaTime, 0f, _maxStat.SP);
            }
        }
    }

    private void OnDestroy()
    {
        LevelChanged = null;
        HPChanged = null;
        MPChanged = null;
        SPChanged = null;
        XPChanged = null;
        StatChanged = null;
        GoldChanged = null;
        SkillPointChanged = null;
    }
}
