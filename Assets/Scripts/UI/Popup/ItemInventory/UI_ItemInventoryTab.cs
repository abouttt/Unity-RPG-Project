using UnityEngine;

public class UI_ItemInventoryTab : MonoBehaviour
{
    [field: SerializeField]
    public ItemType TabType { get; private set; }
    public RectTransform RectTransform { get; private set; }

    private void Awake()
    {
        RectTransform = GetComponent<RectTransform>();
    }
}
