using UnityEngine;

public class Player : MonoBehaviour
{
    public static GameObject GameObject { get; private set; }
    public static Animator Animator { get; private set; }
    public static PlayerStatus Status { get; private set; }
    public static PlayerMovement Movement { get; private set; }
    public static PlayerCameraController Camera { get; private set; }
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
        Camera = GetComponent<PlayerCameraController>();
        ItemInventory = GetComponent<ItemInventory>();
        EquipmentInventory = GetComponent<EquipmentInventory>();
        QuickInventory = GetComponent<QuickInventory>();
        SkillTree = GetComponent<SkillTree>();
    }
}
