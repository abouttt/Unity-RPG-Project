using System.Collections;
using UnityEngine;

public class GameScene : BaseScene
{
    [field: SerializeField, Space(10)]
    public string SceneID { get; private set; }

    [field: SerializeField, Space(10)]
    public Vector3 DefaultSpawnPosition { get; private set; }
    [field: SerializeField]
    public float DefaultSpawnRotationYaw { get; private set; }

    [field: SerializeField, Space(10)]
    public Vector3 PortalSpawnPosition { get; private set; }
    [field: SerializeField]
    public float PortalSpawnRotationYaw { get; private set; }

    protected override void Init()
    {
        base.Init();
    }
}
