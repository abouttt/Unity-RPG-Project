using UnityEngine;

public class EtcItem : CountableItem
{
    public EtcItemData EtcData { get; private set; }

    public EtcItem(EtcItemData data, int count = 1)
        : base(data, count)
    {
        EtcData = data;
    }
}
