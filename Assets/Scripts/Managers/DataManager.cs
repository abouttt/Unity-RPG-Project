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
        SaveItemInventory();
        SaveEquipmentInventory();
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
        PlayerTransformSaveData saveData = new()
        {
            PlayerPosition = Player.GameObject.transform.position,
            PlayerRotationYaw = Player.GameObject.transform.eulerAngles.y,
            CameraPitch = Player.Camera.Pitch,
            CameraYaw = Player.Camera.Yaw,
        };

        SaveToFile(SavePath.PlayerTransformSavePath, JsonUtility.ToJson(saveData));
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
