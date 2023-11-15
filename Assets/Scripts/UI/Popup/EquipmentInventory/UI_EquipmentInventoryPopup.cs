using System;
using System.Collections.Generic;
using UnityEngine;

public class UI_EquipmentInventoryPopup : UI_Popup
{
    enum Buttons
    {
        CloseButton,
    }

    enum Texts
    {
        HPText,
        MPText,
        SPText,
        DamageText,
        DefenseText,
    }

    enum EquipmentSlots
    {
        UI_EquipmentSlot_Helmet,
        UI_EquipmentSlot_Chest,
        UI_EquipmentSlot_Pants,
        UI_EquipmentSlot_Boots,
        UI_EquipmentSlot_Weapon,
        UI_EquipmentSlot_Shield,
    }

    private readonly Dictionary<EquipmentType, UI_EquipmentSlot> _equipmentSlots = new();
    private int _equipmentTypeCount = -1;

    protected override void Init()
    {
        base.Init();

        BindButton(typeof(Buttons));
        BindText(typeof(Texts));
        Bind<UI_EquipmentSlot>(typeof(EquipmentSlots));

        Player.EquipmentInventory.Changed += RefreshSlot;
        //Player.Status.HPChanged += RefreshHPText;
        //Player.Status.MPChanged += RefreshMPText;
        //Player.Status.SPChanged += RefreshSPText;
        //Player.Status.StatChanged += RefreshAllStatusText;

        GetButton((int)Buttons.CloseButton).onClick.AddListener(Managers.UI.Close<UI_EquipmentInventoryPopup>);

        InitSlots();
    }

    private void Start()
    {
        Managers.UI.Register<UI_EquipmentInventoryPopup>(this);

        RefreshAllSlot();
        RefreshAllStatusText();
    }

    private void RefreshSlot(EquipmentType equipmentType)
    {
        if (Player.EquipmentInventory.IsNullSlot(equipmentType))
        {
            _equipmentSlots[equipmentType].Clear();
        }
        else
        {
            _equipmentSlots[equipmentType].SetItem(Player.EquipmentInventory.GetItem(equipmentType));
        }
    }

    private void RefreshAllSlot()
    {
        for (int i = 0; i < _equipmentTypeCount; i++)
        {
            EquipmentType type = (EquipmentType)i;
            RefreshSlot(type);
        }
    }

    private void RefreshAllStatusText()
    {
        //RefreshHPText();
        //RefreshMPText();
        //RefreshSPText();
        //RefreshDamageText();
        //RefreshDefenseText();
    }

    //private void RefreshHPText() => GetText((int)Texts.HPText).text = $"ü�� : {Player.Status.HP} / {Player.Status.MaxStat.HP}";
    //private void RefreshMPText() => GetText((int)Texts.MPText).text = $"���� : {Player.Status.MP} / {Player.Status.MaxStat.MP}";
    //private void RefreshSPText() => GetText((int)Texts.SPText).text = $"��� : {(int)Player.Status.SP} / {Player.Status.MaxStat.SP}";
    //private void RefreshDamageText() => GetText((int)Texts.DamageText).text = $"���ݷ� : {Player.Status.Damage}";
    //private void RefreshDefenseText() => GetText((int)Texts.DefenseText).text = $"���� : {Player.Status.Defense}";

    private void InitSlots()
    {
        var equipmentTypes = Enum.GetValues(typeof(EquipmentType));
        var equipmentSlots = Enum.GetValues(typeof(EquipmentSlots));
        _equipmentTypeCount = equipmentTypes.Length;
        for (int i = 0; i < _equipmentTypeCount; i++)
        {
            var type = (EquipmentType)equipmentTypes.GetValue(i);
            var slot = (EquipmentSlots)equipmentSlots.GetValue(i);
            _equipmentSlots.Add(type, Get<UI_EquipmentSlot>((int)slot));
        }
    }
}
