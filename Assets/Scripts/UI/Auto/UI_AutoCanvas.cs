using UnityEngine;

public class UI_AutoCanvas : UI_Base
{
    [field: SerializeField]
    public UI_InteractionKeyGuide InteractionKeyGuide { get; private set; }
    [field: SerializeField]
    public GameObject LockOnTargetImage { get; private set; }

    protected override void Init()
    {
        Managers.UI.Register<UI_AutoCanvas>(this);

        InteractionKeyGuide.transform.SetParent(transform);
        LockOnTargetImage.transform.SetParent(transform);
    }

    private void Start()
    {
        InteractionKeyGuide.gameObject.SetActive(false);
        LockOnTargetImage.SetActive(false);
    }
}
