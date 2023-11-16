using UnityEngine;

public class UI_QuickInventoryFixed : UI_Base
{
    enum RectTransforms
    {
        QuickSlots,
    }

    private UI_QuickSlot[] _quickSlots;

    protected override void Init()
    {
        BindRT(typeof(RectTransforms));

        InitSlots();

        Player.QuickInventory.InventoryChanged += index => _quickSlots[index].RefreshSlot();
    }

    private void Start()
    {
        Managers.UI.Register<UI_QuickInventoryFixed>(this);
    }

    private void InitSlots()
    {
        for (int i = 0; i < Player.QuickInventory.Capacity; i++)
        {
            var go = Managers.Resource.Instantiate("UI_QuickSlot", GetRT((int)RectTransforms.QuickSlots).transform);
            var quickSlot = go.GetComponent<UI_QuickSlot>();
            quickSlot.InitSlot(i);
        }

        _quickSlots = GetRT((int)RectTransforms.QuickSlots).GetComponentsInChildren<UI_QuickSlot>();
    }
}
