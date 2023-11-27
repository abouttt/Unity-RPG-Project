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

    [field: SerializeField]
    public List<QuestData> Quests { get; private set; }

    private static readonly List<NPC> s_NPCs = new();

    [SerializeField]
    private Vector3 _questNotifierPosition;
    private GameObject _questPresenceNotifier;
    private GameObject _questCompletableNotifier;

    protected override void Awake()
    {
        base.Awake();

        s_NPCs.Add(this);

        Managers.Game.GameStarted += () =>
        {
            Managers.Quest.QuestRegistered += CheckQuest;
            Managers.Quest.QuestCompletabled += CheckQuest;
            Managers.Quest.QuestCompletableCanceled += CheckQuest;
            Managers.Quest.QuestCompleted += CheckQuest;
            Managers.Quest.QuestUnRegistered += CheckQuest;

            _questPresenceNotifier = Managers.Resource.Instantiate("QuestPresenceNotifier", _questNotifierPosition, transform);
            _questCompletableNotifier = Managers.Resource.Instantiate("QuestCompletableNotifier", _questNotifierPosition, transform);
            _questPresenceNotifier.SetActive(false);
            _questCompletableNotifier.SetActive(false);

            CheckQuest();
        };
    }

    public static NPC GetNPC(string id)
    {
        return s_NPCs.Find(npc => npc.NPCID.Equals(id));
    }

    public override void Interaction()
    {
        if (!Conversation && !Shop && !Quest)
        {
            return;
        }

        Managers.Quest.ReceiveReport(Category.NPC, NPCID, 1);
        Managers.UI.Show<UI_NPCMenuPopup>().SetNPC(this);
    }

    private void CheckQuest(Quest quest)
    {
        CheckQuest();
    }

    private void CheckQuest()
    {
        _questPresenceNotifier.SetActive(false);
        _questCompletableNotifier.SetActive(false);

        if (Quests.Count == 0)
        {
            return;
        }

        int unlockQuestCount = 0;
        bool hasCompletableQuest = false;
        foreach (var questData in Quests)
        {
            if (questData.LimitLevel > Player.Status.Level)
            {
                unlockQuestCount++;
                continue;
            }

            var quest = Managers.Quest.GetActiveQuest(questData);
            if (quest is null)
            {
                continue;
            }

            if (quest.State is QuestState.Completable)
            {
                hasCompletableQuest = true;
                break;
            }
        }

        if (Quests.Count != unlockQuestCount)
        {
            _questPresenceNotifier.SetActive(!hasCompletableQuest);
            _questCompletableNotifier.SetActive(hasCompletableQuest);
        }
    }

    private void OnDestroy()
    {
        s_NPCs.Remove(this);
    }
}
