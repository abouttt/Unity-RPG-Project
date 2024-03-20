using UnityEngine;

public class Monster_TrackingState : StateMachineBehaviour
{
    private Monster _monster;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (_monster == null)
        {
            _monster = animator.GetComponent<Monster>();
        }

        _monster.ResetAllTriggers();
        _monster.NaveMeshAgentUpdateToggle(true);
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (Vector3.Distance(Player.Transform.position, _monster.transform.position) > _monster.TrackingDistance)
        {
            _monster.Transition(MonsterState.Restore);
        }
        else if (_monster.IsThePlayerInAttackRange())
        {
            _monster.Transition(MonsterState.Attack);
        }
        else
        {
            _monster.NavMeshAgent.SetDestination(Player.Transform.position);
        }
    }
}
