using UnityEngine;

public class GoblinThrower : Monster
{
    [SerializeField]
    private Transform _rightHand;

    [SerializeField]
    private float _projectileSpeed;

    private void OnAttack()
    {
        if (CurrentState != MonsterState.Attack)
        {
            return;
        }

        var weapon = Managers.Resource.Instantiate(
            "GoblinThrowerThrowingWeapon.prefab", _rightHand.position, transform.rotation * Quaternion.Euler(90f, 0f, 0f), null, true);
        var projectile = weapon.GetComponent<Projectile>();
        projectile.Damage = Data.Damage;
        projectile.Shoot(transform.forward * _projectileSpeed);
    }
}
