using UnityEngine;

public class Monster_AttackState : StateMachineBehaviour
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
        if (stateInfo.normalizedTime < 0.3f)
        {
            Vector3 dir = Player.GameObject.transform.position - _monster.transform.position;
            _monster.transform.rotation = Quaternion.Lerp(
                _monster.transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime * _monster.RotationSmoothTime);
        }
    }
}
