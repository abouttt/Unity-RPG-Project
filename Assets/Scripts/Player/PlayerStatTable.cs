using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Player/Stat Table", fileName = "Player Stat Table")]
public class PlayerStatTable : ScriptableObject
{
    [field: SerializeField]
    public List<PlayerStatData> StatTable { get; private set; }
}
