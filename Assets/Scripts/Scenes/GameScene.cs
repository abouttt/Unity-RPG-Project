using System.Collections;
using UnityEngine;

public class GameScene : BaseScene
{
    [field: SerializeField, Space(10)]
    public string SceneID { get; private set; }

    [field: SerializeField, Space(10)]
    public Vector3 DefaultSpawnPosition { get; private set; }
    [field: SerializeField]
    public float DefaultSpawnRotationYaw { get; private set; }

    [field: SerializeField, Space(10)]
    public Vector3 PortalSpawnPosition { get; private set; }
    [field: SerializeField]
    public float PortalSpawnRotationYaw { get; private set; }

    protected override void Init()
    {
        base.Init();

        Player.Init();
        InitUI();
        StartCoroutine(GameStart());

        Managers.UI.Get<UI_TopCanvas>().FadeInitBG();
        Managers.UI.Get<UI_TopCanvas>().ToggleGameMenuButton(true);
        Managers.Input.ToggleCursor(false);
        Managers.Game.IsDefaultSpawnPosition = false;
        Managers.Game.IsPortalSpawnPosition = false;
        Managers.Quest.ReceiveReport(Category.Scene, SceneID, 1);

        Player.Status.Gold += 10000;
    }

    private void InitUI()
    {
        var UIPackage = Managers.Resource.Instantiate("UIPackage");
        UIPackage.transform.DetachChildren();
        Destroy(UIPackage);
    }

    private IEnumerator GameStart()
    {
        Managers.Quest.Init();
        yield return null;
        Managers.Game.OnGameStarted();
    }
}
