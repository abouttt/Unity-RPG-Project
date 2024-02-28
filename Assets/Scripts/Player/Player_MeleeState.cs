using UnityEngine;

public class Player_MeleeState : StateMachineBehaviour
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Player.Movement.CanMove = false;
        Player.Movement.CanRotation = false;
        Player.Movement.CanJump = false;
        Player.Movement.CanRoll = false;
        Player.Battle.CanAttack = false;
        Player.Battle.CanDefense = false;
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (stateInfo.normalizedTime >= 1f)
        {
            Player.Movement.ClearJumpInfo();
            Player.Movement.ClearRollInfo();
            Player.Movement.CanMove = true;
            Player.Movement.CanRotation = true;
            Player.Movement.CanJump = true;
            Player.Movement.CanRoll = true;
            Player.Battle.ClearBattleInfo();
            Player.Battle.CanAttack = true;
            Player.Battle.CanDefense = true;
        }
    }
}
