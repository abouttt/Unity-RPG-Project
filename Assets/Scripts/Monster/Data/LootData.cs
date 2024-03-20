using System;
using UnityEngine;

[Serializable]
public struct LootData
{
    public ItemData ItemData;
    public IntRange Count;
    public int DropProbability;
}
