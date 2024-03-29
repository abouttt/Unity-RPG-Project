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
                Player.Status.HP = Player.Status.MaxHP;
                Player.Status.MP = Player.Status.MaxMP;
                Managers.Game.IsDefaultSpawn = true;
                Managers.Scene.LoadScene(SceneType.VillageScene);
            }, "확인을 누르시면 마을에서 부활하게 됩니다.", "확인", null, true, false);
        }
    }
}
