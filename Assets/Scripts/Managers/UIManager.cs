using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

public class UIManager
{
    public bool IsOnSelfishPopup { get; private set; }
    public int ActivePopupCount => _activePopups.Count;

    private readonly Dictionary<UIType, Transform> _uiRoots = new();
    private readonly Dictionary<Type, UI_Base> _uiObjects = new();
    private readonly LinkedList<UI_Popup> _activePopups = new();
    private Transform _root;

    public void Init()
    {
        _root = Util.FindOrInstantiate("UI_Root").transform;

        var names = Enum.GetNames(typeof(UIType));
        var values = Enum.GetValues(typeof(UIType));
        for (int i = 0; i < names.Length; i++)
        {
            _uiRoots.Add((UIType)values.GetValue(i), new GameObject($"UI_{names[i]}_Root").transform);
            _uiRoots[(UIType)values.GetValue(i)].SetParent(_root);
        }
    }

    public bool Register<T>() where T : UI_Base
    {
        var ui = Object.FindObjectOfType<T>();
        return Register<T>(ui);
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
            var popup = ui as UI_Popup;
            InitPopup(popup);
            popup.gameObject.SetActive(false);
        }

        ui.transform.SetParent(_uiRoots[ui.UIType]);
        _uiObjects.Add(typeof(T), ui);

        return true;
    }

    public bool UnRegister<T>() where T : UI_Base
    {
        if (_uiObjects.TryGetValue(typeof(T), out var ui))
        {
            if (ui.UIType == UIType.Popup)
            {
                _activePopups.Remove(ui as UI_Popup);
            }

            _uiObjects.Remove(typeof(T));
            return true;
        }
        else
        {
            Debug.Log($"[UIManager/UnRegister] The {typeof(T).Name} no registered.");
            return false;
        }
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

    public bool IsOn<T>() where T : UI_Base
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
            if (ui.gameObject.activeSelf)
            {
                return ui as T;
            }

            if (ui.UIType == UIType.Popup)
            {
                var popup = ui as UI_Popup;
                if (IsOnSelfishPopup && !popup.IgnoreSelfish)
                {
                    return null;
                }

                if (popup.IsSelfish)
                {
                    CloseAll(UIType.Popup);
                    IsOnSelfishPopup = true;
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
                    IsOnSelfishPopup = false;
                }

                _activePopups.Remove(popup);
            }

            ui.gameObject.SetActive(false);
        }
    }

    public void ShowOrClose<T>() where T : UI_Base
    {
        if (IsOn<T>())
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

            IsOnSelfishPopup = false;
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
            if (_activePopups.First.Value.IsSelfish)
            {
                IsOnSelfishPopup = false;
            }

            var popup = _activePopups.First.Value;
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

        _uiRoots.Clear();
        _uiObjects.Clear();
        _activePopups.Clear();
        IsOnSelfishPopup = false;
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
            Managers.Input.ToggleCursor(true);
        };

        popup.Closed += () =>
        {
            if (_activePopups.Count == 0)
            {
                Managers.Input.ToggleCursor(false);
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
