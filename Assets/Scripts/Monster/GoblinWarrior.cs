using UnityEngine;

public class GoblinWarrior : Monster
{
    private void OnAttack()
    {
        int cnt = Physics.OverlapSphereNonAlloc(AttackOffset.position, AttackRadius, PlayerCollider, 1 << LayerMask.NameToLayer("Player"));
        if (cnt != 0)
        {
            Player.Battle.TakeDamage(transform.position, Data.Damage);
        }
    }
}
