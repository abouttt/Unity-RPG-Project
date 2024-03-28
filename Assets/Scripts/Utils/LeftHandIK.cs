using UnityEngine;

public class LeftHandIK : MonoBehaviour
{
    [SerializeField]
    private Transform _leftHand;

    [SerializeField]
    private string _layerName;

    private Animator animator;
    private int _weaponslayerIndex;

    void Awake()
    {
        animator = GetComponent<Animator>();
        _weaponslayerIndex = animator.GetLayerIndex(_layerName);
    }

    private void OnAnimatorIK(int layerIndex)
    {
        if (layerIndex != _weaponslayerIndex)
        {
            return;
        }

        animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1f);
        animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1f);

        animator.SetIKPosition(AvatarIKGoal.LeftHand, _leftHand.position);
        animator.SetIKRotation(AvatarIKGoal.LeftHand, _leftHand.rotation);
    }
}
