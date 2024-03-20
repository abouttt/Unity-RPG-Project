using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct QuestSaveData
{
    public string QuestID;
    public QuestState State;
    public Dictionary<string, int> Targets;
}
