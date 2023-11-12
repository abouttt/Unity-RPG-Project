using UnityEngine;
using UnityEngine.EventSystems;

public class UI_BackgroundCanvas : UI_Base, IPointerClickHandler
{
    protected override void Init()
    {
        Managers.UI.Register<UI_BackgroundCanvas>(this);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Managers.Input.ToggleCursor(false);
    }
}
