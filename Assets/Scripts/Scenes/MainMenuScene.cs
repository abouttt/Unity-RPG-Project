using UnityEngine;

public class MainMenuScene : BaseScene
{
    protected override void Init()
    {
        Managers.Data.LoadGameOption();

        base.Init();

        InitUIPackage("UIPackage_MainMenu");
        Managers.UI.Get<UI_TopCanvas>().FadeInitBG();
        Managers.Input.ToggleCursor(true);
    }
}
