using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : GameControls.IPlayerActions
{
    // Value
    public Vector2 Move;
    public Vector2 Look;

    // Button
    public bool Jump;
    public bool LockOn;

    //Pass Through
    public bool Sprint;

    public bool CursorLocked { get; private set; } = true;

    private GameControls _controls;

    public void Init()
    {
        if (_controls == null)
        {
            _controls = new();
            _controls.Player.AddCallbacks(this);
        }

        _controls.Enable();
    }

    public void ToggleCursor(bool toggle)
    {
        CursorLocked = !toggle;
        Cursor.visible = toggle;
        SetCursorState(CursorLocked);
    }

    public string GetBindingPath(string actionNameOrId, int bindingIndex = 0)
    {
        string key = _controls.FindAction(actionNameOrId).bindings[bindingIndex].path;
        return key.GetLastSlashString().ToUpper();
    }

    public void ResetActions()
    {
        _controls.Player.Jump.Reset();
        _controls.Player.LockOn.Reset();
    }

    public void Clear()
    {
        _controls.Player.Disable();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        Move = context.ReadValue<Vector2>();
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        if (CursorLocked)
        {
            Look = context.ReadValue<Vector2>();
        }
        else
        {
            Look = Vector2.zero;
        }
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        Jump = context.ReadValueAsButton();
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        Sprint = context.ReadValueAsButton();
    }

    public void OnCursorToggle(InputAction.CallbackContext context)
    {
        if (!context.performed)
        {
            return;
        }

        if (Managers.UI.IsOnSelfishPopup)
        {
            return;
        }

        ToggleCursor(CursorLocked);
    }

    public void OnLockOn(InputAction.CallbackContext context)
    {
        if (CursorLocked)
        {
            LockOn = context.ReadValueAsButton();
        }
    }

    public void OnCancel(InputAction.CallbackContext context)
    {
        if (!context.performed)
        {
            return;
        }

        if (Managers.UI.ActivePopupCount > 0)
        {
            Managers.UI.CloseTopPopup();
        }
        else
        {
            //Managers.UI.Show<UI_GameMenuPopup>();
        }
    }

    public void OnItemInventory(InputAction.CallbackContext context)
    {
        ShowOrClosePopup<UI_ItemInventoryPopup>(context);
    }

    public void OnEquipmentInventory(InputAction.CallbackContext context)
    {
        ShowOrClosePopup<UI_EquipmentInventoryPopup>(context);
    }

    private void SetCursorState(bool newState)
    {
        Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
    }

    private void ShowOrClosePopup<T>(InputAction.CallbackContext context) where T : UI_Popup
    {
        if (!context.performed)
        {
            return;
        }

        if (Managers.UI.IsOn<UI_ItemSplitPopup>())
        {
            return;
        }

        Managers.UI.ShowOrClose<T>();
    }
}
