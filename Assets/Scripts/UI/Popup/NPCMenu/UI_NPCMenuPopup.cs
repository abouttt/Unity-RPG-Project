using System.Collections.Generic;
using UnityEngine;

public class UI_NPCMenuPopup : UI_Popup
{
    enum GameObjects
    {
        NPCMenuSubitems,
    }

    enum Texts
    {
        HeaderText
    }

    private readonly List<UI_NPCMenuSubitem> _subitems = new();

    protected override void Init()
    {
        base.Init();

        BindObject(typeof(GameObjects));
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
            Managers.UI.Get<UI_TopCanvas>().ToggleGameMenuButton(false);
        };

        Closed += () =>
        {
            Clear();

            Player.Movement.CanMove = true;
            Player.Movement.CanRotation = true;
            Player.Movement.CanJump = true;
            Managers.UI.Get<UI_TopCanvas>().ToggleGameMenuButton(true);
        };
    }

    public void SetNPC(NPC npc)
    {
        if (npc == null)
        {
            return;
        }

        GetText((int)Texts.HeaderText).text = npc.Name;

        if (npc.Conversation)
        {
            AddSubtiem("대화", () =>
            {
                ToggleMenu(false);
                Managers.UI.Show<UI_ConversationPopup>().SetNPC(npc);
            });
        }

        if (npc.Shop)
        {
            AddSubtiem("상점", () =>
            {
                ToggleMenu(false);
                Managers.UI.Show<UI_ShopPopup>().SetNPCSaleItems(npc);
            });
        }

        AddSubtiem("퀘스트", () =>
        {
            ToggleMenu(false);
            Managers.UI.Show<UI_NPCQuestPopup>().SetNPCQuest(npc);
        });

        AddSubtiem("떠난다", () =>
        {
            Managers.UI.Close<UI_NPCMenuPopup>();
        });
    }

    public void ToggleMenu(bool toggle)
    {
        PopupRT.gameObject.SetActive(toggle);
    }

    private void AddSubtiem(string text, UnityEngine.Events.UnityAction call)
    {
        var go = Managers.Resource.Instantiate("UI_NPCMenuSubitem", GetObject((int)GameObjects.NPCMenuSubitems).transform, true);
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

        _subitems.Clear();
    }
}
