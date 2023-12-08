using UnityEngine;

public class UI_MinimapFixed : UI_Base
{
    [SerializeField]
    private float _height;

    private GameObject _minimapCamera;

    protected override void Init()
    {
        _minimapCamera = GameObject.FindWithTag("MinimapCamera");
    }

    private void Start()
    {
        Managers.UI.Register<UI_MinimapFixed>(this);
    }

    private void LateUpdate()
    {
        var position = Player.GameObject.transform.position;
        position.y = _height;
        var euler = Camera.main.transform.rotation.eulerAngles;
        euler.x = 90f;
        euler.z = 0f;
        _minimapCamera.transform.SetPositionAndRotation(position, Quaternion.Euler(euler));
    }
}
