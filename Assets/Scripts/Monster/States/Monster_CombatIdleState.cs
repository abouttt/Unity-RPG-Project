using UnityEngine;

public class Monster_CombatIdleState : StateMachineBehaviour
{
    private Monster _monster;
    private float _currentAttackDelayTime = 0f;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (_monster == null)
        {
            _monster = animator.GetComponent<Monster>();
        }

        _monster.ResetAllTriggers();
        _monster.NaveMeshAgentUpdateToggle(false);
        _currentAttackDelayTime = 0f;
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (_monster.IsThePlayerInAttackRange())
        {
            _currentAttackDelayTime += Time.deltaTime;
            if (_currentAttackDelayTime >= _monster.AttackDelayTime)
            {
                _monster.Transition(MonsterState.Attack);
            }
        }
        else
        {
            _monster.Transition(MonsterState.Tracking);
        }
    }
}
