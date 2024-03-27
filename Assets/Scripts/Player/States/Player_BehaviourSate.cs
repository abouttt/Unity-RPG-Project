using UnityEngine;

public class Player_BehaviourSate : StateMachineBehaviour
{
    public bool IsLockedBehaviour = true;

    [Range(0f, 1f)]
    public float UnLockTime = 0f;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (IsLockedBehaviour)
        {
            Player.Movement.Enabled = false;
            Player.Battle.Enabled = false;
        }
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (stateInfo.normalizedTime >= UnLockTime)
        {
            Player.Movement.ClearJump();
            Player.Movement.ClearRoll();
            Player.Movement.Enabled = true;

            Player.Battle.Clear();
            Player.Battle.Enabled = true;
        }
    }
}
