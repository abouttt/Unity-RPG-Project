using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_Popup : UI_Base, IPointerDownHandler
{
    public event Action Focused;
    public event Action Showed;
    public event Action Closed;

    [field: SerializeField]
    public bool CanFocus { get; protected set; } = true;

    [field: SerializeField]
    public bool IsSelfish { get; private set; }

    [field: SerializeField]
    public bool IgnoreSelfish { get; private set; }

    [field: SerializeField]
    public RectTransform PopupRT { get; private set; }

    [field: SerializeField]
    public Vector3 DefaultPosition { get; private set; }

    public Canvas Canvas { get; private set; }

    public void SetTop()
    {
        Focused?.Invoke();
    }

    protected override void Init()
    {
        if (PopupRT == null)
        {
            PopupRT = transform.GetChild(0).GetComponent<RectTransform>();
        }

        Canvas = GetComponent<Canvas>();
    }

    public void ClearEvents()
    {
        Focused = null;
        Showed = null;
        Closed = null;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (CanFocus)
        {
            Focused?.Invoke();
        }
    }

    private void OnEnable()
    {
        Showed?.Invoke();
    }

    private void OnDisable()
    {
        Closed?.Invoke();
    }

    private void OnDestroy()
    {
        ClearEvents();
    }
}
