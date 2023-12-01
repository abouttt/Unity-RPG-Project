using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using Newtonsoft.Json.Linq;

public class DataManager
{
    private readonly BinaryFormatter _binaryFormatter = new();

    public void Save()
    {
        SaveItemInventory();
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

    private void SaveItemInventory()
    {
        var root = Player.ItemInventory.GetSaveData();
        SaveToFile(SavePath.ItemInventorySavePath, root.ToString());
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
