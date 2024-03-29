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
            float prevSP = _currentStat.SP;
            _currentStat.SP = Mathf.Clamp(value, 0f, MaxStat.SP);
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
            if (IsMaxLevel || MaxStat.XP == 0)
            {
                return;
            }

            _currentStat.XP = value;

            int level = 0;
            while (_currentStat.XP >= MaxStat.XP)
            {
                _currentStat.XP -= MaxStat.XP;
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

    [ReadOnly]
    public PlayerStatData ExtraFixedStat = new();
    [ReadOnly]
    public PlayerStatData ExtraPerStat = new();
    public PlayerStatData MaxStat { get; private set; } = new();

    [SerializeField]
    private PlayerStatTable _playerStatTable;

    [SerializeField]
    private float _recoverySPDelay;

    [SerializeField]
    private float _recoverySPAmount;

    private readonly PlayerStatData _currentStat = new();
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
            SP = MaxStat.SP;
        }
        else
        {
            FillCurrentAbilityStat();
        }
    }

    private void Start()
    {
        Player.EquipmentInventory.InventoryChanged += (equipmentType) =>
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

        StatChanged?.Invoke();
    }

    private void Update()
    {
        RecoverySP();
    }

    public void RefreshAllStat()
    {
        int level = (IsMaxLevel ? _playerStatTable.StatTable.Count : Level) - 1;
        MaxStat.HP = Util.CalcIncreasePer(_playerStatTable.StatTable[level].HP + ExtraFixedStat.HP, ExtraPerStat.HP);
        MaxStat.MP = Util.CalcIncreasePer(_playerStatTable.StatTable[level].MP + ExtraFixedStat.MP, ExtraPerStat.MP);
        MaxStat.SP = Util.CalcIncreasePer((int)(_playerStatTable.StatTable[level].SP + ExtraFixedStat.SP), (int)ExtraPerStat.SP);
        MaxStat.XP = _playerStatTable.StatTable[level].XP;
        MaxStat.Damage = Util.CalcIncreasePer(_playerStatTable.StatTable[level].Damage + ExtraFixedStat.Damage, ExtraPerStat.Damage);
        MaxStat.Defense = Util.CalcIncreasePer(_playerStatTable.StatTable[level].Defense + ExtraFixedStat.Defense, ExtraPerStat.Defense);

        var types = Enum.GetValues(typeof(EquipmentType));
        foreach (EquipmentType equipmentType in types)
        {
            AddStatByEquipment(equipmentType);
        }
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

    private void AddStatByEquipment(EquipmentType equipmentType)
    {
        if (!Player.EquipmentInventory.IsEquipped(equipmentType))
        {
            return;
        }

        var equipment = Player.EquipmentInventory.GetItem(equipmentType);

        MaxStat.HP += equipment.EquipmentData.HP;
        MaxStat.MP += equipment.EquipmentData.MP;
        MaxStat.SP += equipment.EquipmentData.SP;
        MaxStat.Damage += equipment.EquipmentData.Damage;
        MaxStat.Defense += equipment.EquipmentData.Defense;
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

    private void FillAllStat()
    {
        FillCurrentAbilityStat();
        FillCurrentMeleeStat();
    }

    private void FillCurrentAbilityStat()
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
            _recoverySPDeltaTime = 0f;
            return;
        }

        _recoverySPDeltaTime += Time.deltaTime;
        if (_recoverySPDeltaTime >= _recoverySPDelay)
        {
            if (SP < MaxStat.SP)
            {
                SP += Mathf.Clamp(_recoverySPAmount * Time.deltaTime, 0f, MaxStat.SP);
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
