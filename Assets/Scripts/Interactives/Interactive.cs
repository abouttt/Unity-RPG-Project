using UnityEngine;

public abstract class Interactive : MonoBehaviour
{
    [field: SerializeField]
    public string InteractionMessage { get; private set; }
    [field: SerializeField]
    public Vector3 InteractionKeyGuideDeltaPos { get; private set; }
    [field: SerializeField]
    public bool CanInteraction { get; protected set; } = true;
    [field: SerializeField]
    public string MinimapIconSpriteName { get; protected set; }
    [field: SerializeField]
    public string MinimapIconName { get; protected set; }

    public abstract void Interaction();

    protected virtual void Awake()
    {
        tag = "Interactive";
        Managers.Resource.Instantiate("MinimapIcon", transform).GetComponent<MinimapIcon>().Setup(MinimapIconSpriteName, MinimapIconName);
    }
}
