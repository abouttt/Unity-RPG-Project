using UnityEngine;

public class PortalSpawnData
{
    public Vector3 position;
    public float Yaw;
}

public class GameManager
{
    public bool IsDefaultSpawn { get; set; }
    public bool IsPortalSpawn { get; set; }
    public PortalSpawnData PortalSpawnData { get; private set; } = new();
}
