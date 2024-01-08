using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using Newtonsoft.Json.Linq;

public class DataManager
{
    public static readonly string SavePath = $"{Application.streamingAssetsPath}/Saved";
    public static readonly string SaveFilePath = $"{SavePath}/Saves.json";
    public static readonly string GameOptionSavePath = $"{SavePath}/GameOption.json";

    private JObject _saveDatas;
    private readonly BinaryFormatter _binaryFormatter = new();

    public void Init()
    {
        var directory = new DirectoryInfo(SavePath);
        if (!directory.Exists)
        {
            directory.Create();
        }

        LoadFromFile(SaveFilePath, out var root);
        if (root is not null)
        {
            _saveDatas = JObject.Parse(root);
        }
    }

    public void Save()
    {
        JObject saveData = new()
        {
            { SceneManagerEx.SaveKey, Managers.Scene.GetSaveData() },
            { PlayerMovement.SaveKey, Player.Movement.GetSaveData() },
            { PlayerCameraController.SaveKey, Player.Camera.GetSaveData() },
            { PlayerStatus.SaveKey, Player.Status.GetSaveData() },
            { ItemInventory.SaveKey, Player.ItemInventory.GetSaveData() },
            { EquipmentInventory.SaveKey, Player.EquipmentInventory.GetSaveData() },
            { SkillTree.SaveKey, Player.SkillTree.GetSaveData() },
            { QuickInventory.SaveKey, Player.QuickInventory.GetSaveData() },
            { QuestManager.SaveKey, Managers.Quest.GetSaveData() },
            { UI_QuestPopup.SaveKey, Managers.UI.Get<UI_QuestPopup>().GetSaveData() },
        };

        SaveToFile(SaveFilePath, saveData.ToString());
        SaveGameOption();
    }

    public bool Load<T>(string saveKey, out T data) where T : class
    {
        data = null;

        if (_saveDatas is not null)
        {
            var token = _saveDatas.GetValue(saveKey);
            if (token is not null)
            {
                data = token.ToObject<T>();
            }
        }

        return data is not null;
    }

    public bool HasSaveDatas()
    {
        return File.Exists(SaveFilePath);
    }

    public void ClearSaveDatas()
    {
        var directory = new DirectoryInfo(SavePath);
        foreach (FileInfo file in directory.GetFiles())
        {
            if (file.Name.Equals("GameOption.json"))
            {
                continue;
            }

            file.Delete();
        }

        _saveDatas?.RemoveAll();
    }

    public void LoadGameOption()
    {
        if (!LoadFromFile(GameOptionSavePath, out var json))
        {
            return;
        }

        GameOptionSaveData saveData = JsonUtility.FromJson<GameOptionSaveData>(json);
        Managers.Sound.SetVolume(SoundType.Bgm, saveData.BGMVolume);
        Managers.Sound.SetVolume(SoundType.Effect, saveData.EffectVolume);
        QualitySettings.antiAliasing = saveData.MSAA;
        Application.targetFrameRate = saveData.Frame;
        QualitySettings.vSyncCount = saveData.VSync;
    }

    private void SaveGameOption()
    {
        GameOptionSaveData saveData = new()
        {
            BGMVolume = Managers.Sound.GetVolume(SoundType.Bgm),
            EffectVolume = Managers.Sound.GetVolume(SoundType.Effect),
            MSAA = QualitySettings.antiAliasing,
            Frame = Application.targetFrameRate,
            VSync = QualitySettings.vSyncCount
        };

        SaveToFile(GameOptionSavePath, JsonUtility.ToJson(saveData));
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
