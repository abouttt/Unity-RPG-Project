using UnityEngine;

public abstract class Interactive : MonoBehaviour
{
    [field: SerializeField]
    public string InteractionMessage { get; private set; }
    [field: SerializeField]
    public Vector3 InteractionKeyGuideDeltaPos { get; private set; }
    [field: SerializeField]
    public bool CanInteraction { get; private set; } = true;

    public abstract void Interaction();

    protected virtual void Awake()
    {
        gameObject.tag = "Interactive";
    }
}
