using UnityEngine;

public class Player_DeadState : StateMachineBehaviour
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Player.Movement.Enabled = false;
        Player.Battle.Enabled = false;
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (stateInfo.normalizedTime >= 1f)
        {
            Managers.UI.Show<UI_ConfirmationPopup>().SetEvent(() =>
            {
                Player.Status.HP = Player.Status.MaxStat.HP;
                Player.Status.MP = Player.Status.MaxStat.MP;
                Managers.Game.IsDefaultSpawn = true;
                Managers.Scene.LoadScene(SceneType.VillageScene);
            }, "Ȯ���� �����ø� �������� ��Ȱ�ϰ� �˴ϴ�.", "Ȯ��", null, true, false);
        }
    }
}
