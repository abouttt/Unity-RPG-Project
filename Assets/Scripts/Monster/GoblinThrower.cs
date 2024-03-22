using UnityEngine;

public class GoblinThrower : Monster
{
    [SerializeField]
    private Transform _rightHand;

    private void OnAttack()
    {
        var weapon = Managers.Resource.Instantiate(
            "GoblinThrowerThrowingWeapon.prefab", _rightHand.position, transform.rotation * Quaternion.Euler(90f, 0f, 0f), null, true);
        var projectile = weapon.GetComponent<Projectile>();
        projectile.Damage = Data.Damage;
    }
}
