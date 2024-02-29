using UnityEngine;

public class Player_RollState : StateMachineBehaviour
{
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
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
