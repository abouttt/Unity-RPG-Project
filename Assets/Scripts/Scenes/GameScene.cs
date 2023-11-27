using System.Collections;
using UnityEngine;
using Cinemachine;

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

        InitPlayer();
        InitUI();
        StartCoroutine(GameStart());

        Managers.UI.Get<UI_TopCanvas>().FadeInitBG();
        Managers.UI.Get<UI_TopCanvas>().ToggleGameMenuButton(true);
        Managers.Input.ToggleCursor(false);
        Managers.Game.IsDefaultSpawnPosition = false;
        Managers.Game.IsPortalSpawnPosition = false;
        Managers.Quest.ReceiveReport(Category.Scene, SceneID, 1);

        Player.Status.Gold += 10000;
        Player.Status.XP += 700;
    }

    private void InitPlayer()
    {
        if (Player.GameObject != null)
        {
            return;
        }

        GetPositionAndRotation(out var position, out var yaw);
        Camera.main.transform.position = Vector3.zero;

        var playerPackagePrefab = Managers.Resource.Load<GameObject>("PlayerPackage");
        playerPackagePrefab.FindChild("Player").transform.SetPositionAndRotation(position, Quaternion.Euler(0, yaw, 0));

        var playerPackage = Instantiate(playerPackagePrefab);
        playerPackage.transform.DetachChildren();
        Destroy(playerPackage);
        FindObjectOfType<CinemachineStateDrivenCamera>().transform.SetParent(UnityEngine.Camera.main.transform);
    }

    private void InitUI()
    {
        var UIPackage = Managers.Resource.Instantiate("UIPackage");
        UIPackage.transform.DetachChildren();
        Destroy(UIPackage);
    }

    private IEnumerator GameStart()
    {
        yield return null;

        Managers.Game.OnGameStarted();
    }

    private void GetPositionAndRotation(out Vector3 position, out float yaw)
    {
        position = DefaultSpawnPosition;
        yaw = DefaultSpawnRotationYaw;

        if (Managers.Game.IsPortalSpawnPosition)
        {
            position = PortalSpawnPosition;
            yaw = PortalSpawnRotationYaw;
        }
        //else if (Managers.Data.TryGetSaveData(SavePath.PlayerTransformSavePath, out string json))
        //{
        //    var saveData = JsonUtility.FromJson<PlayerTransformSaveData>(json);
        //    position = saveData.Position;
        //    yaw = saveData.RotationYaw;
        //}
    }
}
