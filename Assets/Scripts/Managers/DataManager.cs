using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using Newtonsoft.Json.Linq;

public class DataManager
{
    public static readonly string SavePath = $"{Application.streamingAssetsPath}/Saved";
    public static readonly string SaveFilePath = $"{SavePath}/Saves.json";
    public static readonly string SaveMetaFilePath = $"{SavePath}/Saves.meta";
    public static readonly string GameOptionSavePath = $"{SavePath}/GameOption.json";

    public bool HasSaveData => File.Exists(SaveFilePath);

    private JObject _saveData;
    private readonly BinaryFormatter _binaryFormatter = new();

    public void Init()
    {
        var directory = new DirectoryInfo(SavePath);
        if (!directory.Exists)
        {
            directory.Create();
        }

        LoadFromFile(SaveFilePath, out var root);
        if (root != null)
        {
            _saveData = JObject.Parse(root);
        }
    }

    public void Save()
    {
        if (Managers.Scene.CurrentScene is GameScene)
        {
            var saveData = new JObject()
            {
                { SceneManagerEx.SaveKey, Managers.Scene.CreateSaveData() },
                { PlayerMovement.SaveKey, Player.Movement.CreateSaveData() },
                { PlayerStatus.SaveKey, Player.Status.CreateSaveData() },
                { ItemInventory.SaveKey, Player.ItemInventory.CreateSaveData() },
                { EquipmentInventory.SaveKey, Player.EquipmentInventory.CreateSaveData() },
                { QuickInventory.SaveKey, Player.QuickInventory.CreateSaveData() },
                { SkillTree.SaveKey, Player.SkillTree.CreateSaveData() },
                { QuestManager.SaveKey, Managers.Quest.CreateSaveData() },
                { UI_QuestPopup.SaveKey, Managers.UI.Get<UI_QuestPopup>().CreateSaveData() },
            };

            SaveToFile(SaveFilePath, saveData.ToString());
        }

        SaveGameOption();
    }

    public bool Load<T>(string saveKey, out T saveData) where T : class
    {
        saveData = null;

        if (_saveData != null)
        {
            var token = _saveData.GetValue(saveKey);
            if (token != null)
            {
                saveData = token.ToObject<T>();
            }
        }

        return saveData != null;
    }

    public void SaveGameOption()
    {
        var gameOptionSaveData = new GameOptionSaveData()
        {
            BGMVolume = Managers.Sound.GetVolume(SoundType.Bgm),
            EffectVolume = Managers.Sound.GetVolume(SoundType.Effect),
            MSAA = QualitySettings.antiAliasing,
            Frame = Application.targetFrameRate,
            VSync = QualitySettings.vSyncCount
        };

        SaveToFile(GameOptionSavePath, JsonUtility.ToJson(gameOptionSaveData));
    }

    public void LoadGameOption()
    {
        if (!LoadFromFile(GameOptionSavePath, out var json))
        {
            return;
        }

        var gameOptionSaveData = JsonUtility.FromJson<GameOptionSaveData>(json);
        Managers.Sound.SetVolume(SoundType.Bgm, gameOptionSaveData.BGMVolume);
        Managers.Sound.SetVolume(SoundType.Effect, gameOptionSaveData.EffectVolume);
        QualitySettings.antiAliasing = gameOptionSaveData.MSAA;
        Application.targetFrameRate = gameOptionSaveData.Frame;
        QualitySettings.vSyncCount = gameOptionSaveData.VSync;
    }

    public void ClearSaveData()
    {
        File.Delete(SaveFilePath);
        File.Delete(SaveMetaFilePath);
        _saveData?.RemoveAll();
    }

    private void SaveToFile(string path, string json)
    {
        using var stream = new FileStream(path, FileMode.Create);
        _binaryFormatter.Serialize(stream, json);
    }

    private bool LoadFromFile(string path, out string json)
    {
        json = null;

        if (File.Exists(path))
        {
            using var stream = new FileStream(path, FileMode.Open);
            json = _binaryFormatter.Deserialize(stream) as string;
            return true;
        }
        else
        {
            Debug.Log($"[DataManater] No have save data : {path}");
        }

        return false;
    }
}
