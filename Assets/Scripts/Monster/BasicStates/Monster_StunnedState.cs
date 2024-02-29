using UnityEngine;
using UnityEngine.Animations;

public class Monster_StunnedState : StateMachineBehaviour
{
    private Monster _monster;
    private GameObject _stunnedEffect;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (_monster == null)
        {
            _monster = animator.GetComponent<Monster>();
        }

        _monster.ResetAllTriggers();
        _monster.NaveMeshAgentUpdateToggle(false);

        var bounds = _monster.Collider.bounds;
        _stunnedEffect = Managers.Resource.Instantiate("StunnedEffect", bounds.center + new Vector3(0.0f, bounds.extents.y, 0.0f), null, true);
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (stateInfo.normalizedTime >= 1f)
        {
            Managers.Resource.Destroy(_stunnedEffect);

            if (_monster.IsThePlayerInAttackRange())
            {
                _monster.Transition(BasicMonsterState.Attack);
            }
            else
            {
                _monster.Transition(BasicMonsterState.Tracking);
            }
        }
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Managers.Resource.Destroy(_stunnedEffect);
    }
}
