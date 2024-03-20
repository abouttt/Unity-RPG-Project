using UnityEngine;

public class UI_InteractionKeyGuide : UI_Auto
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

    private UI_FollowWorldObject _followTarget;

    protected override void Init()
    {
        BindImage(typeof(Images));
        BindText(typeof(Texts));

        _followTarget = GetComponent<UI_FollowWorldObject>();
    }

    public void SetTarget(Interactive target)
    {
        if (target != null)
        {
            _followTarget.SetTarget(target.transform, target.InteractionKeyGuidePos);
            SetText(target);
        }

        gameObject.SetActive(target != null);
    }

    private void SetText(Interactive target)
    {
        GetImage((int)Images.BG).gameObject.SetActive(target.CanInteraction);
        GetText((int)Texts.KeyText).gameObject.SetActive(target.CanInteraction);
        GetText((int)Texts.KeyText).text = Managers.Input.GetBindingPath("Interaction");
        GetText((int)Texts.InteractionText).gameObject.SetActive(target.CanInteraction);
        GetText((int)Texts.InteractionText).text = target.InteractionMessage;

        if (target is NPC npc)
        {
            GetText((int)Texts.NameText).text = npc.NPCName;
            GetText((int)Texts.NameText).gameObject.SetActive(true);
        }
        else
        {
            GetText((int)Texts.NameText).gameObject.SetActive(false);
        }
    }
}
