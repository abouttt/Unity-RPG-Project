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
        Managers.Input.ToggleCursor(false);

        Player.Status.Gold += 10000;

        StartCoroutine(GameStart());
    }

    private void Start()
    {
        Managers.Quest.LoadCleanup();
    }

    private IEnumerator GameStart()
    {
        yield return null;

        Managers.Game.IsDefaultSpawnPosition = false;
        Managers.Game.IsPortalSpawnPosition = false;
        Managers.Quest.ReceiveReport(Category.Scene, SceneID, 1);
        Managers.Sound.Play(_sceneBGM, SoundType.Bgm);
        Managers.UI.Get<UI_TopCanvas>().FadeInitBG();
        Managers.UI.Get<UI_TopCanvas>().ToggleGameMenuButton(true);
    }
}
