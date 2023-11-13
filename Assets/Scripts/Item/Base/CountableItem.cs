using UnityEngine;

public abstract class CountableItem : Item
{
    public CountableItemData CountableData { get; private set; }

    public int CurrentCount { get; private set; }
    public int MaxCount => CountableData.MaxCount;
    public bool IsEmpty => CurrentCount <= 0;
    public bool IsMax => CurrentCount >= CountableData.MaxCount;

    public CountableItem(CountableItemData data, int count)
        : base(data)
    {
        CountableData = data;
        SetCount(count);
    }

    public void SetCount(int count)
    {
        int prevCount = CurrentCount;
        CurrentCount = Mathf.Clamp(count, 0, MaxCount);
        if (prevCount != CurrentCount)
        {
            OnItemChanged();
        }
    }

    public int AddCountAndGetExcess(int count)
    {
        var nextCount = CurrentCount + count;
        SetCount(nextCount);
        return (nextCount > MaxCount) ? (nextCount - MaxCount) : 0;
    }
}
