using UnityEngine;
using Newtonsoft.Json.Linq;

public class Player : MonoBehaviour
{
    public static GameObject GameObject { get; private set; }
    public static Transform Transform { get; private set; }
    public static Collider Collider { get; private set; }
    public static Animator Animator { get; private set; }
    public static PlayerMovement Movement { get; private set; }
    public static PlayerCameraController Camera { get; private set; }
    public static PlayerBattleController Battle { get; private set; }
    public static PlayerStatus Status { get; private set; }
    public static ItemInventory ItemInventory { get; private set; }
    public static EquipmentInventory EquipmentInventory { get; private set; }
    public static QuickInventory QuickInventory { get; private set; }
    public static SkillTree SkillTree { get; private set; }
    public static PlayerRoot Root { get; private set; }
    public static InteractionDetector InteractionDetector { get; private set; }

    private void Awake()
    {
        GameObject = gameObject;
        Transform = transform;
        Collider = gameObject.GetComponent<Collider>();
        Animator = gameObject.GetComponent<Animator>();
        Movement = GetComponent<PlayerMovement>();
        Camera = GetComponent<PlayerCameraController>();
        Battle = GetComponent<PlayerBattleController>();
        Status = GetComponent<PlayerStatus>();
        ItemInventory = GetComponent<ItemInventory>();
        EquipmentInventory = GetComponent<EquipmentInventory>();
        QuickInventory = GetComponent<QuickInventory>();
        SkillTree = GetComponent<SkillTree>();
        Root = GetComponent<PlayerRoot>();
        InteractionDetector = GetComponentInChildren<InteractionDetector>();
    }

    private void Start()
    {
        Managers.Resource.Instantiate("MinimapIcon.prefab", transform)
            .GetComponent<MinimapIcon>().Setup("PlayerMinimapIcon.sprite", "플레이어", 1.2f);
    }

    public static void Init()
    {
        if (GameObject != null)
        {
            return;
        }

        var playerPackagePrefab = Managers.Resource.Load<GameObject>("PlayerPackage.prefab");
        GetPositionAndRotationYaw(out var position, out var yaw);
        playerPackagePrefab.FindChild("Player").transform.SetPositionAndRotation(position, Quaternion.Euler(0, yaw, 0));

        var playerPackage = Instantiate(playerPackagePrefab);
        playerPackage.transform.DetachChildren();
        Destroy(playerPackage);

        playerPackagePrefab.FindChild("Player").transform.SetPositionAndRotation(Vector3.zero, Quaternion.Euler(Vector3.zero));
    }

    private static void GetPositionAndRotationYaw(out Vector3 position, out float yaw)
    {
        position = Vector3.zero;
        yaw = 0f;

        if (Managers.Game.IsDefaultSpawn)
        {
            var gameScene = Managers.Scene.CurrentScene as GameScene;
            position = gameScene.DefaultSpawnPosition;
            yaw = gameScene.DefaultSpawnRotationYaw;
        }
        else if (Managers.Game.IsPortalSpawn)
        {
            position = Managers.Game.PortalSpawnData.position;
            yaw = Managers.Game.PortalSpawnData.Yaw;
        }
        else if (Managers.Data.Load<JArray>(PlayerMovement.SaveKey, out var saveData))
        {
            var transformSaveData = saveData[0].ToObject<TransformSaveData>();
            position = new(transformSaveData.X, transformSaveData.Y, transformSaveData.Z);
            yaw = transformSaveData.RotationYaw;
        }
    }
}
