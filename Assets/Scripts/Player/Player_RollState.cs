using UnityEngine;

public class Player_RollState : StateMachineBehaviour
{
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Player.Movement.ClearRollInfo();
        Player.Movement.CanJump = true;
        Player.Movement.CanRoll = true;
        Player.Battle.ClearBattleInfo();
        Player.Battle.CanAttack = true;
        Player.Battle.CanDefense = true;
    }
}
