using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UI_LootPopup : UI_Popup
{
    enum RectTransforms
    {
        LootSubitems,
    }

    enum Buttons
    {
        CloseButton,
        LootAllButton,
    }

    enum Texts
    {
        LootAllText,
    }

    [SerializeField]
    private float _trackingDistance;
    private FieldItem _fieldItemRef;
    private readonly Dictionary<UI_LootSubitem, FieldItem.Data> _lootSubitems = new();

    protected override void Init()
    {
        base.Init();

        BindRT(typeof(RectTransforms));
        BindButton(typeof(Buttons));
        BindText(typeof(Texts));

        GetButton((int)Buttons.CloseButton).onClick.AddListener(Managers.UI.Close<UI_LootPopup>);
        GetButton((int)Buttons.LootAllButton).onClick.AddListener(AddAllItemToItemInventory);
        GetText((int)Texts.LootAllText).text = $"[{Managers.Input.GetBindingPath("Interaction")}] ¸ðµÎ È¹µæ";

        Managers.UI.Register<UI_LootPopup>(this);
    }

    private void Start()
    {
        Closed += () =>
        {
            Clear();
        };
    }

    private void Update()
    {
        if (_fieldItemRef == null)
        {
            Managers.UI.Close<UI_LootPopup>();
            return;
        }

        TrackingFieldItem();

        if (Managers.Input.Interaction)
        {
            AddAllItemToItemInventory();
        }
    }

    public void SetFieldItem(FieldItem fieldItem)
    {
        _fieldItemRef = fieldItem;

        for (int i = 0; i < fieldItem.Items.Count; i++)
        {
            if (fieldItem.Items[i] is not null)
            {
                AddLootSubitem(fieldItem.Items[i], i);
            }
        }
    }

    public void AddItemToItemInventory(UI_LootSubitem lootSubitem)
    {
        var fieldItemData = _lootSubitems[lootSubitem];
        lootSubitem.FieldItemDataRef.Count = Player.ItemInventory.AddItem(fieldItemData.ItemData, fieldItemData.Count);
        if (lootSubitem.FieldItemDataRef.Count > 0)
        {
            lootSubitem.RefreshCountText();
        }
        else
        {
            RemoveLootSubitem(lootSubitem);
        }
    }

    private void AddLootSubitem(FieldItem.Data fieldItemData, int index)
    {
        var go = Managers.Resource.Instantiate("UI_LootSubitem", GetRT((int)RectTransforms.LootSubitems), true);
        var lootSubitem = go.GetComponent<UI_LootSubitem>();
        lootSubitem.SetFieldItemData(fieldItemData, index);
        _lootSubitems.Add(lootSubitem, fieldItemData);
    }

    private void RemoveLootSubitem(UI_LootSubitem lootSubitem)
    {
        _fieldItemRef.RemoveItem(lootSubitem.Index);
        _lootSubitems.Remove(lootSubitem);
        Managers.Resource.Destroy(lootSubitem.gameObject);
        if (_lootSubitems.Count == 0)
        {
            Managers.UI.Close<UI_LootPopup>();
        }
    }

    private void TrackingFieldItem()
    {
        var distance = Vector3.Distance(Player.GameObject.transform.position, _fieldItemRef.transform.position);
        if (distance >= _trackingDistance)
        {
            Managers.UI.Close<UI_LootPopup>();
        }
    }

    private void AddAllItemToItemInventory()
    {
        for (int i = _lootSubitems.Count - 1; i >= 0; i--)
        {
            AddItemToItemInventory(_lootSubitems.ElementAt(i).Key);
        }
    }

    private void Clear()
    {
        foreach (var lootItem in _lootSubitems)
        {
            Managers.Resource.Destroy(lootItem.Key.gameObject);
        }

        _lootSubitems.Clear();
        _fieldItemRef = null;
    }
}
