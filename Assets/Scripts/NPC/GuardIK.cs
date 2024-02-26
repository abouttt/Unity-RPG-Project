using UnityEngine;

public class GuardIK : MonoBehaviour
{
    public Transform LeftHand;

    private Animator _animator;
    private int _layerIntexWeapons;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _layerIntexWeapons = _animator.GetLayerIndex("Weapons");
    }

    private void OnAnimatorIK(int layerIndex)
    {
        if (layerIndex != _layerIntexWeapons)
        {
            return;
        }

        _animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1f);
        _animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1f);

        _animator.SetIKPosition(AvatarIKGoal.LeftHand, LeftHand.position);
        _animator.SetIKRotation(AvatarIKGoal.LeftHand, LeftHand.rotation);
    }
}
