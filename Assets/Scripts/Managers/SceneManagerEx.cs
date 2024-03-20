using UnityEngine;
using UnityEngine.SceneManagement;
using Newtonsoft.Json.Linq;

public class SceneManagerEx : ISavable
{
    public static string SaveKey => "SaveScene";

    public BaseScene CurrentScene { get { return Object.FindObjectOfType<BaseScene>(); } }
    public SceneType NextScene { get; private set; } = SceneType.Unknown;
    public SceneType PrevScene { get; private set; } = SceneType.Unknown;

    public void LoadScene(SceneType scene)
    {
        if (CurrentScene is GameScene && Managers.Resource.HasResources)
        {
            Managers.Data.Save();
        }

        NextScene = scene;
        PrevScene = CurrentScene.SceneType;
        SceneManager.LoadScene(SceneType.LoadingScene.ToString());
    }

    public JArray CreateSaveData()
    {
        var saveData = new JArray();

        var sceneSaveData = new SceneSaveData()
        {
            Scene = Managers.Scene.CurrentScene.SceneType
        };

        saveData.Add(JObject.FromObject(sceneSaveData));

        return saveData;
    }

    public void LoadSaveData()
    {
    }
}
