using System.Collections;
using UnityEngine;

public class GameScene : BaseScene
{
    [field: SerializeField, Space(10)]
    public string SceneID { get; private set; }
    [SerializeField]
    private AudioClip _sceneBGM;

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
        InitUIPackage("UIPackage_Game");
        StartCoroutine(GameStart());

        Managers.Input.ToggleCursor(false);

        Player.Status.Gold += 10000;
    }

    private IEnumerator GameStart()
    {
        Managers.Game.OnResourceLoaded();
        yield return null;
        Managers.Quest.Init();
        Managers.Quest.ReceiveReport(Category.Scene, SceneID, 1);
        Managers.Game.OnGameStarted();
        Managers.Game.IsDefaultSpawnPosition = false;
        Managers.Game.IsPortalSpawnPosition = false;
        yield return null;
        Managers.Sound.Play(_sceneBGM, SoundType.Bgm);
        Managers.UI.Get<UI_TopCanvas>().FadeInitBG();
        Managers.UI.Get<UI_TopCanvas>().ToggleGameMenuButton(true);
    }
}
