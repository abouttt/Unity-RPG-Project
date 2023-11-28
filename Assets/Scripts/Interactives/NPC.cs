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

    [field: SerializeField, TextArea, Space(10)]
    public List<string> ConversationScripts { get; private set; }

    [field: SerializeField]
    public List<ItemData> SaleItems { get; private set; }

    public IReadOnlyList<QuestData> Quests => _quests;

    private static readonly List<NPC> s_NPCs = new();

    [field: SerializeField]
    private List<QuestData> _quests = new();

    [SerializeField]
    private Vector3 _questNotifierPosition = new Vector3(0f, 2.3f, 0f);
    private GameObject _questPresenceNotifier;
    private GameObject _questCompletableNotifier;

    protected override void Awake()
    {
        base.Awake();

        s_NPCs.Add(this);

        Managers.Game.GameStarted += () =>
        {
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
        Managers.UI.Show<UI_NPCMenuPopup>().SetNPC(this);
        Managers.Quest.ReceiveReport(Category.NPC, NPCID, 1);
    }

    public void AddQuest(QuestData questData)
    {
        _quests.Add(questData);
        CheckQuest();
    }

    public void RemoveQuest(QuestData questData)
    {
        _quests.Remove(questData);
        CheckQuest();
    }

    private void CheckQuest()
    {
        _questPresenceNotifier.SetActive(false);
        _questCompletableNotifier.SetActive(false);

        int lockedQuestCount = 0;
        bool hasCompletableQuest = false;
        foreach (var questData in Quests)
        {
            if (questData.LimitLevel > Player.Status.Level)
            {
                lockedQuestCount++;
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

        if (Quests.Count != lockedQuestCount)
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
