using UnityEngine;
using DG.Tweening;

public class MainMenuScene : BaseScene
{
    protected override void Init()
    {
        base.Init();

        Managers.Data.LoadGameOption();

        LoadResourcesAsync(SceneType, () =>
        {
            InstantiatePackage("UIPackage_MainMenu.prefab");
            Managers.Input.CursorLocked = false;
            GameObject.Find("InitBG").GetComponent<DOTweenAnimation>().DOPlay();
        });
    }
}
