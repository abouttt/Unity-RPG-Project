using UnityEngine;

public class Player_DeadState : StateMachineBehaviour
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

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (stateInfo.normalizedTime >= 1f)
        {
            Managers.UI.Show<UI_ConfirmationPopup>().SetEvent(() =>
            {
                Player.Status.HP = Player.Status.MaxStat.HP;
                Player.Status.MP = Player.Status.MaxStat.MP;
                Managers.Game.IsDefaultSpawnPosition = true;
                Managers.Scene.LoadScene(SceneType.VillageScene);
            }, "Ȯ���� �����ø� �������� ��Ȱ�ϰ� �˴ϴ�.", "Ȯ��", "", true, false);
        }
    }
}
