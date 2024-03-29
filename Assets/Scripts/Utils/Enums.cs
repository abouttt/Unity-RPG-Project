using UnityEngine;

public enum MonsterState
{
    Idle,
    Tracking,
    Restore,
    Attack,
    Damaged,
    Stunned,
    Death,
}

public enum Category
{
    Scene,
    Item,
    Skill,
    NPC,
    Monster,
    Quest,
}

public enum QuestState
{
    Inactive,
    Active,
    Completable,
    Complete,
}

public enum SlotType
{
    Item,
    Equipment,
    Skill,
    Quick,
    Shop,
    QuestReward,
}

public enum SkillType
{
    Active,
    Passive,
}

public enum ItemType
{
    Equipment,
    Consumable,
    Etc,
}

public enum ItemQuality
{
    Low,
    Normal,
    Rare,
    Legendary,
}

public enum EquipmentType
{
    Helmet,
    Chest,
    Pants,
    Boots,
    Weapon,
    Shield,
    Count,
}

public enum UIType
{
    Subitem = -1,
    Background,
    Auto,
    Fixed,
    Popup,
    Top = 1000,
}

public enum SoundType
{
    Bgm,
    Effect,
    UI,
}

public enum AddressableLabel
{
    Default,
    MainMenu_Prefab,
    Game_Prefab,
    Game_UI,
}

public enum SceneType
{
    Unknown,
    MainMenuScene,
    LoadingScene,
    VillageScene,
    DungeonScene,
}
