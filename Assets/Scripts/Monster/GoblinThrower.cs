using UnityEngine;

public class GoblinThrower : Monster
{
    [SerializeField]
    private Transform _rightHand;

    private void OnAttack()
    {
        var weapon = Managers.Resource.Instantiate("GoblinThrowerThrowingWeapon.prefab", _rightHand.position, null, true);
        weapon.transform.LookAt(PlayerCollider[0].bounds.center);
        weapon.GetComponent<Projectile>().Damage = Data.Damage;
    }
}
