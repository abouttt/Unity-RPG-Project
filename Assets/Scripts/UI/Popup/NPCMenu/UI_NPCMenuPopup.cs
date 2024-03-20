using System.Collections.Generic;
using UnityEngine;

public class UI_NPCMenuPopup : UI_Popup
{
    enum RectTransforms
    {
        NPCMenuSubitems,
    }

    enum Texts
    {
        HeaderText
    }

    private NPC _npc;
    private readonly List<UI_NPCMenuSubitem> _subitems = new();

    protected override void Init()
    {
        base.Init();

        BindRT(typeof(RectTransforms));
        BindText(typeof(Texts));
    }

    private void Start()
    {
        Managers.UI.Register<UI_NPCMenuPopup>(this);

        Showed += () =>
        {
            Player.Movement.CanMove = false;
            Player.Movement.CanRotation = false;
            Player.Movement.CanJump = false;
            Player.Movement.CanRoll = false;
            Managers.UI.Get<UI_TopCanvas>().ToggleGameMenuButton(false);
        };

        Closed += () =>
        {
            Clear();

            Player.Movement.CanMove = true;
            Player.Movement.CanRotation = true;
            Player.Movement.CanJump = true;
            Player.Movement.CanRoll = true;
            Managers.UI.Get<UI_TopCanvas>().ToggleGameMenuButton(true);
        };
    }

    public void SetNPC(NPC npc)
    {
        if (npc == null)
        {
            return;
        }

        _npc = npc;

        GetText((int)Texts.HeaderText).text = npc.NPCName;

        if (npc.Conversation)
        {
            AddSubitem("대화", () =>
            {
                PopupRT.gameObject.SetActive(false);
                Managers.UI.Show<UI_ConversationPopup>().SetNPC(npc);
            });
        }

        if (npc.Shop)
        {
            AddSubitem("상점", () =>
            {
                PopupRT.gameObject.SetActive(false);
                Managers.UI.Show<UI_ShopPopup>().SetNPCSaleItems(npc);
            });
        }

        AddSubitem("퀘스트", () =>
        {
            PopupRT.gameObject.SetActive(false);
            Managers.UI.Show<UI_NPCQuestPopup>().SetNPC(npc);
        });

        AddSubitem("떠난다", () =>
        {
            Managers.UI.Close<UI_NPCMenuPopup>();
        });
    }

    private void AddSubitem(string text, UnityEngine.Events.UnityAction call)
    {
        var go = Managers.Resource.Instantiate("UI_NPCMenuSubitem.prefab", GetRT((int)RectTransforms.NPCMenuSubitems), true);
        var subitem = go.GetComponent<UI_NPCMenuSubitem>();
        subitem.SetEvent(text, call);
        _subitems.Add(subitem);
    }

    private void Clear()
    {
        foreach (var subitem in _subitems)
        {
            subitem.Clear();
            Managers.Resource.Destroy(subitem.gameObject);
        }

        _npc.IsInteracted = false;
        _npc = null;
        _subitems.Clear();
    }
}
