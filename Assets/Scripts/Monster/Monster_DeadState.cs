using UnityEngine;

public class Monster_DeadState : StateMachineBehaviour
{
    private Monster _monster;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (_monster == null)
        {
            _monster = animator.GetComponent<Monster>();
        }

        _monster.ResetAllTriggers();
        _monster.NaveMeshAgentUpdateToggle(false);
        _monster.Collider.isTrigger = true;

        if (_monster.IsLockOnTarget)
        {
            Player.Camera.LockOnTarget = null;
        }

        Player.Status.XP += _monster.Stat.GetXP();
        Player.Status.Gold += _monster.Stat.GetGold();
        Managers.Quest.ReceiveReport(Category.Monster, _monster.Stat.MonsterID, 1);
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (stateInfo.length <= stateInfo.normalizedTime)
        {
            Managers.Resource.Destroy(_monster.gameObject);
        }
    }
}
