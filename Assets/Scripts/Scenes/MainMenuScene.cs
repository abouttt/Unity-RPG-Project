using UnityEngine;

public class MainMenuScene : BaseScene
{
    protected override void Init()
    {
        base.Init();

        Managers.Data.LoadGameOption();

        LoadResourcesAsync(SceneType, () =>
        {
            InitUIPackage("UIPackage_MainMenu.prefab");
            Managers.UI.Get<UI_TopCanvas>().FadeInitBG();
            Managers.Input.ToggleCursor(true);
        });
    }
}
