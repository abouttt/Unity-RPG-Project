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

        Managers.Resource.Instantiate("MinimapIcon", transform).GetComponent<MinimapIcon>().Setup("PlayerMinimapIcon", "플레이어");
    }

    public static void Init()
    {
        if (GameObject != null)
        {
            return;
        }

        UnityEngine.Camera.main.transform.position = Vector3.zero;

        var playerPackagePrefab = Managers.Resource.Load<GameObject>("PlayerPackage");
        var playerPackage = Instantiate(playerPackagePrefab);
        playerPackage.transform.DetachChildren();
        Destroy(playerPackage);
        FindObjectOfType<CinemachineStateDrivenCamera>().transform.SetParent(UnityEngine.Camera.main.transform);
    }
}
