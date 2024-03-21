using UnityEngine;

public class GoblinThrower : Monster
{
    [SerializeField]
    private Transform _rightHand;

    private void OnAttack()
    {
        var weapon = Managers.Resource.Instantiate(
            "GoblinThrowerThrowingWeapon.prefab", _rightHand.position, transform.rotation, null, true);
        weapon.GetComponent<Projectile>().Damage = Data.Damage;
    }
}
