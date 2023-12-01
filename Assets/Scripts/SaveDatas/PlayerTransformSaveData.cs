using System;
using UnityEngine;

[Serializable]
public struct PlayerTransformSaveData
{
    public Vector3 PlayerPosition;
    public float PlayerRotationYaw;
    public float CameraPitch;
    public float CameraYaw;
}
