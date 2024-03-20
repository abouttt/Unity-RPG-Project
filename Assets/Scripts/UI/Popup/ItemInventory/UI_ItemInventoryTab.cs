using UnityEngine;

public class UI_ItemInventoryTab : MonoBehaviour
{
    [field: SerializeField]
    public ItemType TabType { get; private set; }
    public RectTransform SlotsRT { get; private set; }

    private void Awake()
    {
        SlotsRT = GetComponent<RectTransform>();
    }
}
