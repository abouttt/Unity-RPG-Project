using UnityEngine;

public abstract class UI_Auto : UI_Base
{
    private void Start()
    {
        transform.SetParent(Managers.UI.Get<UI_AutoCanvas>().transform);
    }
}
