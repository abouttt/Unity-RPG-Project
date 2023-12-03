using System;

[Serializable]
public struct QuestSaveData
{
    public string QuestID;
    public QuestState State;
    public int[] Counts;
}
