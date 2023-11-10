using UnityEngine;

public class MainMenuScene : BaseScene
{
    protected override void Init()
    {
        base.Init();

        Managers.Input.ToggleCursor(true);
    }
}
