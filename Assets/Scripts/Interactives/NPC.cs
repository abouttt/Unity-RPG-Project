using System.Collections.Generic;
using UnityEngine;

public class NPC : Interactive
{
    [field: SerializeField]
    public string NPCID { get; private set; }
    [field: SerializeField]
    public string Name { get; private set; }
    [field: SerializeField]
    public bool Conversation { get; private set; }
    [field: SerializeField]
    public bool Shop { get; private set; }
    [field: SerializeField]
    public bool Quest { get; private set; }

    [field: SerializeField, TextArea, Space(10)]
    public List<string> ConversationScripts { get; private set; }

    [field: SerializeField]
    public List<ItemData> SaleItems { get; private set; }

    //[field: SerializeField]
    //public List<QuestData> Quests { get; private set; }

    private static readonly List<NPC> s_npcs = new();

    protected override void Awake()
    {
        base.Awake();
        s_npcs.Add(this);
    }

    public static NPC GetNPC(string id)
    {
        return s_npcs.Find(npc => npc.NPCID == id);
    }

    public override void Interaction()
    {
        if (!Conversation && !Shop && !Quest)
        {
            return;
        }

        Managers.UI.Show<UI_NPCMenuPopup>().SetNPC(this);
    }

    private void OnDestroy()
    {
        s_npcs.Remove(this);
    }
}
