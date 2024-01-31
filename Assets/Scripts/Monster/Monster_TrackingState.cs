using UnityEngine;

public class Monster_TrackingState : StateMachineBehaviour
{
    private Monster _monster;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (_monster == null)
        {
            _monster = animator.GetComponent<Monster>();
        }

        _monster.NavMeshAgent.isStopped = false;
        _monster.NavMeshAgent.updateRotation = true;
        _monster.ResetAllTriggers();
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (Vector3.Distance(Player.GameObject.transform.position, _monster.transform.position) > _monster.TrackingDistance)
        {
            _monster.Animator.SetTrigger(_monster.AnimIDIdle);
        }
        else if (_monster.IsThePlayerInAttackRange())
        {
            
        }
        else
        {
            _monster.NavMeshAgent.SetDestination(Player.GameObject.transform.position);
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _monster.NavMeshAgent.isStopped = true;
        _monster.NavMeshAgent.updateRotation = false;
        _monster.NavMeshAgent.velocity = Vector3.zero;
    }
}
