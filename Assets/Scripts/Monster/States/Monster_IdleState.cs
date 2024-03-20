using UnityEngine;

public class Monster_IdleState : StateMachineBehaviour
{
    private Monster _monster;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (_monster == null)
        {
            _monster = animator.GetComponent<Monster>();
        }

        _monster.ResetAllTriggers();
        _monster.NaveMeshAgentUpdateToggle(false);
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (_monster.PlayerDetect())
        {
            if (_monster.IsThePlayerInAttackRange())
            {
                _monster.Transition(MonsterState.Attack);
            }
            else
            {
                _monster.Transition(MonsterState.Tracking);
            }
        }
    }
}
