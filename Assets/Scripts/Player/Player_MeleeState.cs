using UnityEngine;

public class Player_MeleeState : StateMachineBehaviour
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Player.Battle.CanAttack = false;
        Player.Battle.CanDefense = false;
        Player.Movement.CanMove = false;
        Player.Movement.CanRotation = false;
        Player.Movement.CanJump = false;
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (stateInfo.normalizedTime >= 1f)
        {
            Player.Battle.ClearAttackInfo();
            Player.Battle.CanAttack = true;
            Player.Battle.CanDefense = true;
            Player.Movement.CanMove = true;
            Player.Movement.CanRotation = true;
            Player.Movement.CanJump = true;
        }
    }
}
