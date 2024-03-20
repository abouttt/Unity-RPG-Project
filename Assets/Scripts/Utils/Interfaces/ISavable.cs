using UnityEngine;
using Newtonsoft.Json.Linq;

public interface ISavable
{
    public static string SaveKey { get; }
    public JArray CreateSaveData();
    public void LoadSaveData();
}
