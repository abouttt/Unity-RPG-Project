using UnityEngine;

public class Portal : Interactive
{
    [SerializeField]
    private SceneType _loadScene;

    [SerializeField]
    private Vector3 _position;

    [SerializeField]
    private float _yaw;

    private void Start()
    {
        InstantiateMinimapIcon("PortalMinimapIcon.sprite", "Æ÷Å»");
    }

    public override void Interaction()
    {
        Managers.Game.IsPortalSpawn = true;
        Managers.Game.PortalSpawnData.position = _position;
        Managers.Game.PortalSpawnData.Yaw = _yaw;
        Managers.Scene.LoadScene(_loadScene);
    }
}
