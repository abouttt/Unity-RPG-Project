using UnityEngine;

public class UI_LootSubitem : UI_Base
{
    enum Images
    {
        ItemImage,
    }

    enum Texts
    {
        ItemNameText,
        CountText,
    }

    enum Buttons
    {
        LootButton,
    }

    public FieldItem.Data FieldItemDataRef { get; private set; }
    public int Index { get; private set; } = -1;

    protected override void Init()
    {
        BindImage(typeof(Images));
        BindText(typeof(Texts));
        BindButton(typeof(Buttons));

        GetButton((int)Buttons.LootButton).onClick.AddListener(() =>
        {
            UI_LootPopup lootPopup = Managers.UI.Get<UI_LootPopup>();
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
            GetText((int)Texts.CountText).gameObject.SetActive(true);
            GetText((int)Texts.CountText).text = $"x{FieldItemDataRef.Count}";
        }
        else
        {
            GetText((int)Texts.CountText).gameObject.SetActive(false);
        }
    }
}
