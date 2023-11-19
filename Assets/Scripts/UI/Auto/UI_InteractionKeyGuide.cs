using UnityEngine;

public class UI_InteractionKeyGuide : UI_Base
{
    enum Images
    {
        BG,
    }

    enum Texts
    {
        KeyText,
        InteractionText,
        NameText,
    }

    private UI_FollowTarget _followTarget;

    protected override void Init()
    {
        BindImage(typeof(Images));
        BindText(typeof(Texts));
        _followTarget = GetComponent<UI_FollowTarget>();
    }

    public void SetTarget(Interactive target)
    {
        if (target != null)
        {
            _followTarget.SetTarget(target.transform, target.InteractionKeyGuideDeltaPos);
            SetText(target);
        }

        gameObject.SetActive(target != null);
    }

    private void SetText(Interactive target)
    {
        GetImage((int)Images.BG).enabled = target.CanInteraction;
        GetText((int)Texts.KeyText).enabled = target.CanInteraction;
        GetText((int)Texts.InteractionText).enabled = target.CanInteraction;
        GetText((int)Texts.KeyText).text = Managers.Input.GetBindingPath("Interaction");
        GetText((int)Texts.InteractionText).text = target.InteractionMessage;

        //if (target is NPC npc)
        //{
        //    GetText((int)Texts.NameText).text = npc.Name;
        //    GetText((int)Texts.NameText).enabled = true;
        //}
        //else
        //{
        //    GetText((int)Texts.NameText).enabled = false;
        //}
    }
}
