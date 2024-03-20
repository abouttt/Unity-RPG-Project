using UnityEngine;

public class UI_AutoCanvas : UI_Base
{
    protected override void Init()
    {
        Managers.UI.Register<UI_AutoCanvas>(this);
    }

    private void Start()
    {
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }
    }
}
