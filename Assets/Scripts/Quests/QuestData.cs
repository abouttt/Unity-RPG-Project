using UnityEngine;
using AYellowpaper.SerializedCollections;

[CreateAssetMenu(menuName = "Quest/QuestData", fileName = "Quest_")]
public class QuestData : ScriptableObject
{
    [field: SerializeField]
    public string QuestID { get; private set; }

    [field: SerializeField]
    public string QuestName { get; private set; }

    [field: SerializeField, TextArea]
    public string Description { get; private set; }

    [field: SerializeField]
    public int LimitLevel { get; private set; }

    [field: SerializeField]
    public string OwnerID { get; private set; }

    [field: SerializeField]
    public string CompleteOwnerID { get; private set; }

    [field: SerializeField]
    public bool CanRemoteComplete { get; private set; }

    [field: SerializeField, Header("Target")]
    public QuestTarget[] Targets { get; private set; }

    [Header("Reward")]
    public int RewardGold;
    public int RewardXP;
    public SerializedDictionary<ItemData, int> RewardItems;

    public bool Equals(QuestData other)
    {
        if (other == null)
        {
            return false;
        }

        if (GetType() != other.GetType())
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return QuestID.Equals(other.QuestID);
    }
}
