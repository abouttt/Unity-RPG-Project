using UnityEngine;

public class UI_LootSubitem : UI_Base
{
    enum Images
    {
        ItemImage,
    }

    enum Buttons
    {
        LootButton,
    }

    enum Texts
    {
        ItemNameText,
        CountText,
    }

    public FieldItem.Data FieldItemDataRef { get; private set; }
    public int Index { get; private set; } = -1;

    protected override void Init()
    {
        BindImage(typeof(Images));
        BindButton(typeof(Buttons));
        BindText(typeof(Texts));

        GetButton((int)Buttons.LootButton).onClick.AddListener(() =>
        {
            var lootPopup = Managers.UI.Get<UI_LootPopup>();
            lootPopup.SetTop();
            lootPopup.AddItemToItemInventory(this);
        });
    }

    public void SetFieldItemData(FieldItem.Data fieldItemData, int index)
    {
        FieldItemDataRef = fieldItemData;
        Index = index;
        GetImage((int)Images.ItemImage).sprite = FieldItemDataRef.ItemData.ItemImage;
        GetText((int)Texts.ItemNameText).text = FieldItemDataRef.ItemData.ItemName;
        RefreshCountText();
    }

    public void RefreshCountText()
    {
        if (FieldItemDataRef.ItemData is CountableItemData && FieldItemDataRef.Count > 1)
        {
            GetText((int)Texts.CountText).enabled = true;
            GetText((int)Texts.CountText).text = $"x{FieldItemDataRef.Count}";
        }
        else
        {
            GetText((int)Texts.CountText).enabled = false;
        }
    }
}
