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
            Player.Movement.CanMove = false;
            Player.Movement.CanRotation = false;
            Player.Movement.CanJump = false;
            Player.Movement.CanRoll = false;
            Player.Battle.CanAttack = false;
            Player.Battle.CanParry = false;
            Player.Battle.CanDefense = false;
        }
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (stateInfo.normalizedTime >= UnLockTime)
        {
            Player.Movement.ClearJump();
            Player.Movement.ClearRoll();
            Player.Movement.CanMove = true;
            Player.Movement.CanRotation = true;
            Player.Movement.CanJump = true;
            Player.Movement.CanRoll = true;
            Player.Battle.Clear();
            Player.Battle.CanAttack = true;
            Player.Battle.CanParry = true;
            Player.Battle.CanDefense = true;
        }
    }
}
