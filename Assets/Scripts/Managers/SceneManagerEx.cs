using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagerEx
{
    public BaseScene CurrentScene { get { return Object.FindObjectOfType<BaseScene>(); } }
    public SceneType NextScene { get; private set; } = SceneType.Unknown;
    public SceneType PrevScene { get; private set; } = SceneType.Unknown;

    public void LoadScene(SceneType scene)
    {
        if (CurrentScene is GameScene)
        {
            Managers.Data.Save();
        }

        NextScene = scene;
        PrevScene = CurrentScene.SceneType;
        SceneManager.LoadScene(SceneType.LoadingScene.ToString());
    }
}
