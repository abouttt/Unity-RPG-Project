using UnityEngine;
using Cinemachine;

public class Player : MonoBehaviour
{
    public static GameObject GameObject { get; private set; }
    public static Animator Animator { get; private set; }
    public static PlayerStatus Status { get; private set; }
    public static PlayerMovement Movement { get; private set; }
    public static PlayerCameraController Camera { get; private set; }
    public static PlayerRoot Root { get; private set; }
    public static PlayerBattleController Battle { get; private set; }
    public static ItemInventory ItemInventory { get; private set; }
    public static EquipmentInventory EquipmentInventory { get; private set; }
    public static QuickInventory QuickInventory { get; private set; }
    public static SkillTree SkillTree { get; private set; }

    private void Awake()
    {
        GameObject = gameObject;
        Animator = GetComponent<Animator>();
        Status = GetComponent<PlayerStatus>();
        Movement = GetComponent<PlayerMovement>();
        Root = GetComponent<PlayerRoot>();
        Battle = GetComponent<PlayerBattleController>();
        Camera = GetComponent<PlayerCameraController>();
        ItemInventory = GetComponent<ItemInventory>();
        EquipmentInventory = GetComponent<EquipmentInventory>();
        QuickInventory = GetComponent<QuickInventory>();
        SkillTree = GetComponent<SkillTree>();

        Managers.Resource.Instantiate("MinimapIcon", transform).GetComponent<MinimapIcon>().Setup("PlayerMinimapIcon", "플레이어", 1.3f);
    }

    public static void Init()
    {
        if (GameObject != null)
        {
            return;
        }

        UnityEngine.Camera.main.transform.position = Vector3.zero;

        var playerPackagePrefab = Managers.Resource.Load<GameObject>("PlayerPackage");
        GetPositionAndRotationYaw(out var position, out var yaw);
        playerPackagePrefab.FindChild("Player").transform.SetPositionAndRotation(position, Quaternion.Euler(0, yaw, 0));

        var playerPackage = Instantiate(playerPackagePrefab);
        playerPackage.transform.DetachChildren();
        Destroy(playerPackage);
        FindObjectOfType<CinemachineStateDrivenCamera>().transform.SetParent(UnityEngine.Camera.main.transform);
        Movement.InitRotationYaw = yaw;

        playerPackagePrefab.FindChild("Player").transform.SetPositionAndRotation(Vector3.zero, Quaternion.Euler(Vector3.zero));
    }

    private static void GetPositionAndRotationYaw(out Vector3 position, out float yaw)
    {
        var gameScene = Managers.Scene.CurrentScene as GameScene;
        position = Vector3.zero;
        yaw = 0f;

        if (Managers.Game.IsDefaultSpawnPosition)
        {
            position = gameScene.DefaultSpawnPosition;
            yaw = gameScene.DefaultSpawnRotationYaw;
        }
        else if (Managers.Game.IsPortalSpawnPosition)
        {
            position = gameScene.PortalSpawnPosition;
            yaw = gameScene.PortalSpawnRotationYaw;
        }
        else if (Managers.Data.Load<string>(PlayerMovement.SaveKey, out var json))
        {
            var saveData = JsonUtility.FromJson<TransformSaveData>(json);
            position = saveData.Position;
            yaw = saveData.RotationYaw;
        }
    }
}
