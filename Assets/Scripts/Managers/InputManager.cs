using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : GameControls.IPlayerActions
{
    // Value
    public Vector2 Move;
    public Vector2 Look;

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

    private void SetCursorState(bool newState)
    {
        Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
    }
}
