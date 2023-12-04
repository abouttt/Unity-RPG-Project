using UnityEngine;

public static class SavePath
{
    public readonly static string Path = $"{Application.streamingAssetsPath}/SaveData";
    public readonly static string SceneSavePath = $"{Path}/Scene.json";
    public readonly static string TransformSavePath = $"{Path}/Transform.json";
    public readonly static string CameraSavePath = $"{Path}/Camera.json";
    public readonly static string StatusSavePath = $"{Path}/Status.json";
    public readonly static string GameOptionSavePath = $"{Path}/GameOption.json";
    public readonly static string ItemInventorySavePath = $"{Path}/ItemInventory.json";
    public readonly static string EquipmentInventorySavePath = $"{Path}/EquipmentInventory.json";
    public readonly static string QuestSavePath = $"{Path}/Quest.json";
    public readonly static string QuestUISavePath = $"{Path}/QuestUI.json";
    public readonly static string QuickBarSavePath = $"{Path}/QuickBar.json";
    public readonly static string SkillTreeSavePath = $"{Path}/SkillTree.json";
}
