using UnityEngine;

public class MainMenuScene : BaseScene
{
    protected override void Init()
    {
        base.Init();

        Managers.UI.Get<UI_TopCanvas>().FadeInitBG();
        Managers.Input.ToggleCursor(true);
    }
}
