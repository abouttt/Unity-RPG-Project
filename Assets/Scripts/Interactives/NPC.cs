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

    public IReadOnlyList<string> ConversationScripts => _conversationScripts;
    public IReadOnlyList<ItemData> SaleItems => _saleItems;
    public IReadOnlyList<QuestData> Quests => _quests;

    private static readonly Dictionary<string, NPC> s_NPCs = new();

    [field: SerializeField, TextArea, Space(10)]
    private List<string> _conversationScripts;
    [field: SerializeField]
    private List<ItemData> _saleItems;
    [field: SerializeField, ReadOnly]
    private List<QuestData> _quests;

    [SerializeField]
    private Vector3 _questNotifierPosition = new(0f, 2.3f, 0f);
    private GameObject _questPresenceNotifier;
    private GameObject _questCompletableNotifier;

    private bool _originCanInteraction;

    protected override void Awake()
    {
        MinimapIconName = Name;
        MinimapIconSpriteName = "NPCMinimapIcon";

        base.Awake();

        s_NPCs.Add(NPCID, this);
        _quests = QuestDatabase.GetInstance.FindQuestsBy(NPCID);

        _questPresenceNotifier = Managers.Resource.Instantiate("QuestPresenceNotifier", _questNotifierPosition, transform);
        _questCompletableNotifier = Managers.Resource.Instantiate("QuestCompletableNotifier", _questNotifierPosition, transform);
        _questPresenceNotifier.SetActive(false);
        _questCompletableNotifier.SetActive(false);

        _originCanInteraction = CanInteraction;
    }

    private void Start()
    {
        CheckQuests();
    }

    public static NPC GetNPC(string id)
    {
        return s_NPCs.TryGetValue(id, out NPC npc) ? npc : null;
    }

    public override void Interaction()
    {
        Managers.UI.Show<UI_NPCMenuPopup>().SetNPC(this);
        Managers.Quest.ReceiveReport(Category.NPC, NPCID, 1);
    }

    public void AddQuest(QuestData questData)
    {
        _quests.Add(questData);
        CheckQuests();
    }

    public void RemoveQuest(QuestData questData)
    {
        _quests.Remove(questData);
        CheckQuests();
    }

    private void CheckQuests()
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

        if (!CanInteraction && (_questPresenceNotifier.activeSelf || _questCompletableNotifier.activeSelf))
        {
            CanInteraction = true;
        }
        else if (CanInteraction && !_originCanInteraction)
        {
            CanInteraction = false;
        }
    }

    private void OnDestroy()
    {
        s_NPCs.Remove(NPCID);
    }
}
