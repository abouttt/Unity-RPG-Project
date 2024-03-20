using System;
using UnityEngine;

[Serializable]
public struct GameOptionSaveData
{
    public float BGMVolume;
    public float EffectVolume;
    public int MSAA;
    public int Frame;
    public int VSync;
}
