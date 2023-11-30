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

    [SerializeField]
    private SerializedDictionary<EquipmentType, EquipData> _equipDatas;
    [SerializeField]
    private RuntimeAnimatorController _basicAnimator;
    [SerializeField]
    private AnimatorOverrideController _battleAnimator;

    private void Start()
    {
        Player.EquipmentInventory.EquipmentChanged += RefreshEquipmentObject;

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

    public bool IsEquip(EquipmentType equipmentType)
    {
        if (_equipDatas.TryGetValue(equipmentType, out var equipRootData))
        {
            return equipRootData.Equipment != null;
        }

        return false;
    }

    private void RefreshEquipmentObject(EquipmentType equipmentType)
    {
        if (!_equipDatas.TryGetValue(equipmentType, out _))
        {
            return;
        }

        var equipData = _equipDatas[equipmentType];

        if (Player.EquipmentInventory.IsNullSlot(equipmentType))
        {
            Destroy(equipData.Equipment);
            equipData.Equipment = null;
        }
        else
        {
            if (equipData.Equipment != null)
            {
                Destroy(equipData.Equipment);
            }

            var equipment = Instantiate(Player.EquipmentInventory.GetItem(equipmentType).EquipmentData.ItemPrefab, equipData.Root);
            equipData.Equipment = equipment;
        }

        RefreshAnimator();
    }

    private void RefreshAnimator()
    {
        bool hasEquipment = false;

        if (_equipDatas[EquipmentType.Weapon].Equipment != null ||
            _equipDatas[EquipmentType.Shield].Equipment != null)
        {
            hasEquipment = true;
        }

        Player.Animator.runtimeAnimatorController = hasEquipment ? _battleAnimator : _basicAnimator;

        if (Player.Camera.IsLockOn)
        {
            Player.Camera.LockOnTarget = Player.Camera.LockOnTarget;
        }
    }
}
