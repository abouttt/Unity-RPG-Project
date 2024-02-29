using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_ItemInventoryPopup : UI_Popup
{
    enum RectTransforms
    {
        ItemInventory,
        EquipmentSlots,
        ConsumableSlots,
        EtcSlots
    }

    enum Buttons
    {
        CloseButton,
    }

    enum Texts
    {
        GoldText
    }

    enum Tabs
    {
        EquipmentTabButton,
        ConsumableTabButton,
        EtcTabButton,
    }

    enum ScrollRects
    {
        ItemSlotScrollView,
    }

    [SerializeField]
    private float _tabXOffset;
    [SerializeField]
    private float _tabClickedXPosition;

    private readonly Dictionary<ItemType, UI_ItemSlot[]> _slots = new();
    private readonly Dictionary<UI_ItemInventoryTab, RectTransform> _tabs = new();

    protected override void Init()
    {
        base.Init();

        BindRT(typeof(RectTransforms));
        BindButton(typeof(Buttons));
        BindText(typeof(Texts));
        Bind<UI_ItemInventoryTab>(typeof(Tabs));
        Bind<ScrollRect>(typeof(ScrollRects));

        GetButton((int)Buttons.CloseButton).onClick.AddListener(Managers.UI.Close<UI_ItemInventoryPopup>);

        Player.ItemInventory.InventoryChanged += (itemType, index) =>
        {
            switch (itemType)
            {
                case ItemType.Equipment:
                    RefreshSlot(ItemType.Equipment, index);
                    break;
                case ItemType.Consumable:
                    RefreshSlot(ItemType.Consumable, index);
                    break;
                case ItemType.Etc:
                    RefreshSlot(ItemType.Etc, index);
                    break;
            }
        };

        Player.Status.GoldChanged += () => GetText((int)Texts.GoldText).text = Player.Status.Gold.ToString();

        InitSlots();
        InitTabs();

        RefreshAllSlots(ItemType.Equipment);
        RefreshAllSlots(ItemType.Consumable);
        RefreshAllSlots(ItemType.Etc);
    }

    private void Start()
    {
        Managers.UI.Register<UI_ItemInventoryPopup>(this);

        Showed += () =>
        {
            GetRT((int)RectTransforms.ItemInventory).SetParent(transform);
            Get<ScrollRect>((int)ScrollRects.ItemSlotScrollView).verticalScrollbar.value = 1f;
        };

        OpenTheTab(ItemType.Equipment);
    }

    public void OpenTheTab(ItemType itemType)
    {
        Get<ScrollRect>((int)ScrollRects.ItemSlotScrollView).verticalScrollbar.value = 1f;

        foreach (var tab in _tabs)
        {
            var pos = tab.Key.RectTransform.anchoredPosition;

            if (tab.Key.TabType == itemType)
            {
                Get<ScrollRect>((int)ScrollRects.ItemSlotScrollView).content = tab.Value;
                pos.x = _tabClickedXPosition;
                tab.Value.gameObject.SetActive(true);
            }
            else
            {
                pos.x = _tabXOffset;
                tab.Value.gameObject.SetActive(false);
            }

            tab.Key.RectTransform.anchoredPosition = pos;
        }
    }

    public void ToggleCloseButton(bool toggle)
    {
        GetButton((int)Buttons.CloseButton).gameObject.SetActive(toggle);
    }

    private void RefreshSlot(ItemType itemType, int index)
    {
        if (Player.ItemInventory.IsNullSlot(itemType, index))
        {
            _slots[itemType][index].Clear();
        }
        else
        {
            _slots[itemType][index].SetItem(Player.ItemInventory.GetItem<Item>(itemType, index));
        }
    }

    private void RefreshAllSlots(ItemType itemType)
    {
        for (int i = 0; i < _slots[itemType].Length; i++)
        {
            RefreshSlot(itemType, i);
        }
    }

    private void InitSlots()
    {
        CreateSlots(Player.ItemInventory.Inventories[ItemType.Equipment].Capacity, GetRT((int)RectTransforms.EquipmentSlots));
        CreateSlots(Player.ItemInventory.Inventories[ItemType.Consumable].Capacity, GetRT((int)RectTransforms.ConsumableSlots));
        CreateSlots(Player.ItemInventory.Inventories[ItemType.Etc].Capacity, GetRT((int)RectTransforms.EtcSlots));

        _slots.Add(ItemType.Equipment, GetRT((int)RectTransforms.EquipmentSlots).GetComponentsInChildren<UI_ItemSlot>());
        _slots.Add(ItemType.Consumable, GetRT((int)RectTransforms.ConsumableSlots).GetComponentsInChildren<UI_ItemSlot>());
        _slots.Add(ItemType.Etc, GetRT((int)RectTransforms.EtcSlots).GetComponentsInChildren<UI_ItemSlot>());
    }

    private void InitTabs()
    {
        _tabs.Add(Get<UI_ItemInventoryTab>((int)Tabs.EquipmentTabButton), GetRT((int)RectTransforms.EquipmentSlots));
        _tabs.Add(Get<UI_ItemInventoryTab>((int)Tabs.ConsumableTabButton), GetRT((int)RectTransforms.ConsumableSlots));
        _tabs.Add(Get<UI_ItemInventoryTab>((int)Tabs.EtcTabButton), GetRT((int)RectTransforms.EtcSlots));

        foreach (var tab in _tabs)
        {
            tab.Key.GetComponent<Button>().onClick.AddListener(() =>
            {
                SetTop();
                OpenTheTab(tab.Key.TabType);
            });
        }
    }

    private void CreateSlots(int capacity, Transform parent)
    {
        for (int i = 0; i < capacity; i++)
        {
            var go = Managers.Resource.Instantiate("UI_ItemSlot.prefab", parent);
            go.GetComponent<UI_ItemSlot>().Index = i;
        }
    }
}
