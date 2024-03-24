using UnityEngine;

public abstract class Interactive : MonoBehaviour
{
    public bool IsSensed { get; set; } = false;     // 감지 되었는지
    public bool IsInteracted { get; set; } = false; // 상호작용 중인지

    [field: SerializeField]
    public string InteractionMessage { get; protected set; }

    [field: SerializeField]
    public Vector3 InteractionKeyGuidePos { get; protected set; }

    [field: SerializeField]
    public bool CanInteraction { get; protected set; } = true;

    public abstract void Interaction();

    protected virtual void Awake()
    {
        gameObject.layer = LayerMask.NameToLayer("Interactive");
    }

    protected void InstantiateMinimapIcon(string spriteName, string iconName, float scale = 1)
    {
        Managers.Resource.Instantiate(
            "MinimapIcon.prefab", transform).GetComponent<MinimapIcon>().Setup(spriteName, iconName, scale);
    }
}
