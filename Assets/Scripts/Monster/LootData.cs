using System;

[Serializable]
public struct LootData
{
    public ItemData ItemData;
    public int DropProbability;
    public IntRange Count;
}
