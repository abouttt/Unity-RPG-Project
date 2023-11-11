using UnityEngine;

public class UI_AutoCanvas : UI_Base
{
    [field: SerializeField]
    public GameObject LockOnTargetImage { get; private set; }

    protected override void Init()
    {
        Managers.UI.Register<UI_AutoCanvas>(this);
        LockOnTargetImage.transform.SetParent(transform);
    }

    private void Start()
    {
        LockOnTargetImage.SetActive(false);
    }
}
