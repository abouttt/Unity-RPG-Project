using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using Newtonsoft.Json.Linq;

public class DataManager
{
    private readonly BinaryFormatter _binaryFormatter = new();

    public void Save()
    {
        SaveScene();
        SavePlayerTransform();
        SaveCamera();
        SaveStatus();
        SaveItemInventory();
        SaveEquipmentInventory();
        SaveSkillTree();
        SaveQuickInventory();
        SaveQuest();
        SaveGameOption();
    }

    public bool HasSaveDatas()
    {
        var directory = new DirectoryInfo(SavePath.Path);
        if (!directory.Exists)
        {
            directory.Create();
        }

        return directory.GetFiles().Length > 0;
    }

    public bool TryGetSaveData(string path, out string json)
    {
        if (!LoadFromFile(path, out json))
        {
            return false;
        }

        return true;
    }

    public bool TryGetSaveData(string path, out JObject data)
    {
        if (!LoadFromFile(path, out var json))
        {
            data = null;
            return false;
        }

        data = JObject.Parse(json);
        return true;
    }

    public void ClearSaveDatas()
    {
        var directory = new DirectoryInfo(SavePath.Path);
        foreach (FileInfo file in directory.GetFiles())
        {
            if (file.Name.Equals("GameOptionSaveData"))
            {
                continue;
            }

            file.Delete();
        }
    }

    public SceneType LoadScene()
    {
        if (!LoadFromFile(SavePath.SceneSavePath, out var json))
        {
            return SceneType.Unknown;
        }

        SceneSaveData saveData = JsonUtility.FromJson<SceneSaveData>(json);
        return saveData.Scene;
    }

    public void LoadGameOption()
    {
        if (!LoadFromFile(SavePath.GameOptionSavePath, out var json))
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

    private void SaveScene()
    {
        SceneSaveData saveData = new()
        {
            Scene = Managers.Scene.CurrentScene.SceneType
        };

        SaveToFile(SavePath.SceneSavePath, JsonUtility.ToJson(saveData));
    }

    private void SavePlayerTransform()
    {
        TransformSaveData saveData = new()
        {
            Position = Player.GameObject.transform.position,
            RotationYaw = Player.GameObject.transform.eulerAngles.y,
        };

        SaveToFile(SavePath.TransformSavePath, JsonUtility.ToJson(saveData));
    }

    private void SaveCamera()
    {
        CameraSaveData saveData = new()
        {
            Pitch = Player.Camera.Pitch,
            Yaw = Player.Camera.Yaw,
        };

        SaveToFile(SavePath.CameraSavePath, JsonUtility.ToJson(saveData));
    }

    private void SaveStatus()
    {
        StatusSaveData saveData = new()
        {
            Level = Player.Status.Level,
            CurrentHP = Player.Status.HP,
            CurrentMP = Player.Status.MP,
            CurrentXP = Player.Status.XP,
            Gold = Player.Status.Gold,
            SkillPoint = Player.Status.SkillPoint,
        };

        SaveToFile(SavePath.StatusSavePath, JsonUtility.ToJson(saveData));
    }

    private void SaveItemInventory()
    {
        var root = Player.ItemInventory.GetSaveData();
        SaveToFile(SavePath.ItemInventorySavePath, root.ToString());
    }

    private void SaveEquipmentInventory()
    {
        var root = Player.EquipmentInventory.GetSaveData();
        SaveToFile(SavePath.EquipmentInventorySavePath, root.ToString());
    }

    private void SaveSkillTree()
    {
        var root = Player.SkillTree.GetSaveData();
        SaveToFile(SavePath.SkillTreeSavePath, root.ToString());
    }

    private void SaveQuickInventory()
    {
        var root = Player.QuickInventory.GetSaveData();
        SaveToFile(SavePath.QuickBarSavePath, root.ToString());
    }

    private void SaveQuest()
    {
        var root = Managers.Quest.GetSaveData();
        SaveToFile(SavePath.QuestSavePath, root.ToString());
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

        SaveToFile(SavePath.GameOptionSavePath, JsonUtility.ToJson(saveData));
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
