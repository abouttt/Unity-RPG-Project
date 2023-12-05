using System.IO;

#if UNITY_EDITOR
using UnityEditor;

public static class EditorMenuItems
{
    [MenuItem("Tools/Clear Save Data")]
    public static void ClearSaveData()
    {
        var directory = new DirectoryInfo(DataManager.SavePath);
        if (!directory.Exists)
        {
            directory.Create();
        }

        foreach (FileInfo file in directory.GetFiles())
        {
            file.Delete();
        }
    }
}

#endif
