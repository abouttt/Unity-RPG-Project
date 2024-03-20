using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

public class UIManager
{
    public int ActivePopupCount => _activePopups.Count;
    public bool IsShowedSelfishPopup { get; private set; }

    private readonly Dictionary<UIType, Transform> _uiRoots = new();
    private readonly Dictionary<Type, UI_Base> _uiObjects = new();
    private readonly LinkedList<UI_Popup> _activePopups = new();
    private Transform _root;

    public void Init()
    {
        _root = Util.FindOrInstantiate("UI_Root").transform;

        var names = Enum.GetNames(typeof(UIType));
        var types = Enum.GetValues(typeof(UIType));
        for (int i = 0; i < names.Length; i++)
        {
            _uiRoots.Add((UIType)types.GetValue(i), new GameObject($"UI_{names[i]}_Root").transform);
            _uiRoots[(UIType)types.GetValue(i)].SetParent(_root);
        }
    }

    public bool Register<T>(UI_Base ui) where T : UI_Base
    {
        if (ui == null)
        {
            Debug.Log($"[UIManager/Register] {typeof(T).Name} object is null.");
            return false;
        }

        if (ui is not T)
        {
            Debug.Log($"[UIManager/Register] {ui.name} is not {typeof(T)}");
            return false;
        }

        if (_uiObjects.ContainsKey(typeof(T)))
        {
            Debug.Log($"[UIManager/Register] {ui.name} is already registered.");
            return false;
        }

        if (!ui.TryGetComponent<Canvas>(out var canvas))
        {
            Debug.Log($"[UIManager/Register] {ui.name} is no exist Canvas component.");
            return false;
        }
        else
        {
            canvas.sortingOrder = (int)ui.UIType;
        }

        if (ui.UIType == UIType.Popup)
        {
            UI_Popup popup = ui as UI_Popup;
            InitPopup(popup);
            popup.gameObject.SetActive(false);
        }

        ui.transform.SetParent(_uiRoots[ui.UIType]);
        _uiObjects.Add(typeof(T), ui);

        return true;
    }

    public bool UnRegister<T>() where T : UI_Base
    {
        bool isRegistered = _uiObjects.TryGetValue(typeof(T), out var ui);
        if (isRegistered)
        {
            if (ui is UI_Popup popup)
            {
                _activePopups.Remove(popup);
                popup.ClearEvents();
            }

            _uiObjects.Remove(typeof(T));
        }
        else
        {
            Debug.Log($"[UIManager/UnRegister] The {typeof(T).Name} no registered.");
        }

        return isRegistered;
    }

    public T Get<T>() where T : UI_Base
    {
        if (_uiObjects.TryGetValue(typeof(T), out var ui))
        {
            return ui as T;
        }

        return null;
    }

    public Transform GetRoot(UIType uiType)
    {
        return _uiRoots[uiType];
    }

    public bool IsShowed<T>() where T : UI_Base
    {
        if (_uiObjects.TryGetValue(typeof(T), out var ui))
        {
            return ui.gameObject.activeSelf;
        }

        return false;
    }

    public T Show<T>() where T : UI_Base
    {
        if (_uiObjects.TryGetValue(typeof(T), out var ui))
        {
            if (ui.UIType == UIType.Popup)
            {
                var popup = ui as UI_Popup;

                if (IsShowedSelfishPopup && !popup.IgnoreSelfish)
                {
                    return null;
                }

                if (popup.IsSelfish)
                {
                    CloseAll(UIType.Popup);
                    IsShowedSelfishPopup = true;
                }

                _activePopups.AddFirst(popup);
                RefreshAllPopupDepth();
            }

            ui.gameObject.SetActive(true);
            return ui as T;
        }

        return null;
    }

    public void Close<T>() where T : UI_Base
    {
        if (_uiObjects.TryGetValue(typeof(T), out var ui))
        {
            if (ui.UIType == UIType.Popup)
            {
                var popup = ui as UI_Popup;

                if (popup.IsSelfish)
                {
                    IsShowedSelfishPopup = false;
                }

                _activePopups.Remove(popup);
            }

            ui.gameObject.SetActive(false);
        }
    }

    public void ShowOrClose<T>() where T : UI_Base
    {
        if (IsShowed<T>())
        {
            Close<T>();
        }
        else
        {
            Show<T>();
        }
    }

    public void CloseAll(UIType type)
    {
        if (type == UIType.Popup)
        {
            foreach (var popup in _activePopups)
            {
                popup.gameObject.SetActive(false);
            }

            IsShowedSelfishPopup = false;
            _activePopups.Clear();
        }
        else
        {
            foreach (var element in _uiObjects)
            {
                if (element.Value.UIType == type)
                {
                    element.Value.gameObject.SetActive(false);
                }
            }
        }
    }

    public void CloseTopPopup()
    {
        if (ActivePopupCount > 0)
        {
            var popup = _activePopups.First.Value;

            if (popup.IsSelfish)
            {
                IsShowedSelfishPopup = false;
            }

            _activePopups.RemoveFirst();
            popup.gameObject.SetActive(false);
        }
    }

    public void Clear()
    {
        if (_root != null)
        {
            foreach (Transform child in _root)
            {
                Object.Destroy(child.gameObject);
            }
        }

        IsShowedSelfishPopup = false;
        _uiRoots.Clear();
        _uiObjects.Clear();
        _activePopups.Clear();
    }

    private void InitPopup(UI_Popup popup)
    {
        popup.PopupRT.anchoredPosition = popup.DefaultPosition;

        popup.Focused += () =>
        {
            _activePopups.Remove(popup);
            _activePopups.AddFirst(popup);
            RefreshAllPopupDepth();
        };

        popup.Showed += () =>
        {
            Managers.Input.CursorLocked = false;
        };

        popup.Closed += () =>
        {
            if (_activePopups.Count == 0)
            {
                Managers.Input.CursorLocked = true;
            }
        };
    }

    private void RefreshAllPopupDepth()
    {
        int count = 1;
        foreach (var popup in _activePopups)
        {
            popup.Canvas.sortingOrder = (int)UIType.Top - count++;
        }
    }
}
