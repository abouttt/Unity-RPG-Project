using System;
using UnityEngine;
using AYellowpaper.SerializedCollections;

public class PlayerRoot : MonoBehaviour
{
    [Serializable]
    public class EquipData
    {
        public Transform Root;
        [ReadOnly]
        public GameObject Equipment;
    }

    public Action<EquipmentType> EquipmentChanged;

    [SerializeField]
    private SerializedDictionary<EquipmentType, EquipData> _equipDatas;

    [SerializeField]
    private RuntimeAnimatorController _basicAnimator;

    [SerializeField]
    private AnimatorOverrideController _meleeAnimator;

    private void Awake()
    {
        Player.EquipmentInventory.InventoryChanged += RefreshEquipmentObject;
    }

    private void Start()
    {
        var types = Enum.GetValues(typeof(EquipmentType));
        foreach (EquipmentType type in types)
        {
            RefreshEquipmentObject(type);
        }
    }

    public Transform GetRoot(EquipmentType equipmentType)
    {
        if (_equipDatas.TryGetValue(equipmentType, out var data))
        {
            return data.Root;
        }

        return null;
    }

    public GameObject GetEquipment(EquipmentType equipmentType)
    {
        if (_equipDatas.TryGetValue(equipmentType, out var data))
        {
            return data.Equipment;
        }

        return null;
    }

    private void RefreshEquipmentObject(EquipmentType equipmentType)
    {
        if (!_equipDatas.TryGetValue(equipmentType, out _))
        {
            return;
        }

        var equipData = _equipDatas[equipmentType];

        if (Player.EquipmentInventory.IsEquipped(equipmentType))
        {
            if (equipData.Equipment != null)
            {
                Destroy(equipData.Equipment);
            }

            var equipment = Instantiate(Player.EquipmentInventory.GetItem(equipmentType).EquipmentData.ItemPrefab, equipData.Root);
            equipData.Equipment = equipment;
        }
        else
        {
            Destroy(equipData.Equipment);
            equipData.Equipment = null;
        }

        RefreshAnimator();
        EquipmentChanged?.Invoke(equipmentType);
    }

    private void RefreshAnimator()
    {
        bool hasEquipment = false;

        if (Player.EquipmentInventory.IsEquipped(EquipmentType.Weapon) ||
            Player.EquipmentInventory.IsEquipped(EquipmentType.Shield))
        {
            hasEquipment = true;
        }

        Player.Animator.runtimeAnimatorController = hasEquipment ? _meleeAnimator : _basicAnimator;

        if (Player.Camera.IsLockOn)
        {
            Player.Camera.LockOnTarget = Player.Camera.LockOnTarget;
        }
    }
}
