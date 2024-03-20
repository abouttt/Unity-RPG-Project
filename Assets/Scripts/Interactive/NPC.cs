using System.Collections.Generic;
using UnityEngine;

public class NPC : Interactive
{
    [field: SerializeField]
    public string NPCID { get; private set; }

    [field: SerializeField]
    public string NPCName { get; private set; }

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

    [SerializeField]
    private Vector3 _questNotifierInteractionPosition = new(0f, 2.7f, 0f);

    private GameObject _questPresenceNotifier;
    private GameObject _questCompletableNotifier;

    private bool _originCanInteraction;

    private void Awake()
    {
        s_NPCs.Add(NPCID, this);
        _quests = QuestDatabase.GetInstance.FindQuestsBy(NPCID);
        _originCanInteraction = CanInteraction;
        InteractionMessage = "¥Î»≠";
    }

    private void Start()
    {
        InstantiateMinimapIcon("NPCMinimapIcon.sprite", NPCName);
        _questPresenceNotifier = Managers.Resource.Instantiate("QuestPresenceNotifier.prefab", _questNotifierPosition, transform);
        _questCompletableNotifier = Managers.Resource.Instantiate("QuestCompletableNotifier.prefab", _questNotifierPosition, transform);
        _questPresenceNotifier.SetActive(false);
        _questCompletableNotifier.SetActive(false);

        CheckQuests();
    }

    public static NPC GetNPC(string id)
    {
        return s_NPCs.TryGetValue(id, out var npc) ? npc : null;
    }

    public static bool TryAddQuestToNPC(string id, QuestData questData)
    {
        if (s_NPCs.TryGetValue(id, out var npc))
        {
            npc._quests.Add(questData);
            npc.CheckQuests();
            return true;
        }

        return false;
    }

    public static bool TryRemoveQuestToNPC(string id, QuestData questData)
    {
        if (s_NPCs.TryGetValue(id, out var npc))
        {
            npc._quests.Remove(questData);
            npc.CheckQuests();
            return true;
        }

        return false;
    }

    public override void Interaction()
    {
        Managers.UI.Show<UI_NPCMenuPopup>().SetNPC(this);
        Managers.Quest.ReceiveReport(Category.NPC, NPCID, 1);
    }

    private void OnTriggerStay(Collider other)
    {
        if (!IsSensed)
        {
            return;
        }

        if (Player.InteractionDetector.IsKeyGuideShowed)
        {
            _questPresenceNotifier.transform.localPosition = _questNotifierInteractionPosition;
            _questCompletableNotifier.transform.localPosition = _questNotifierInteractionPosition;
        }
        else
        {
            _questPresenceNotifier.transform.localPosition = _questNotifierPosition;
            _questCompletableNotifier.transform.localPosition = _questNotifierPosition;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        _questPresenceNotifier.transform.localPosition = _questNotifierPosition;
        _questCompletableNotifier.transform.localPosition = _questNotifierPosition;
    }

    private void CheckQuests()
    {
        _questPresenceNotifier.SetActive(false);
        _questCompletableNotifier.SetActive(false);

        int lockedQuestCount = 0;
        bool hasCompletableQuest = false;

        foreach (var questData in _quests)
        {
            if (questData.LimitLevel > Player.Status.Level)
            {
                lockedQuestCount++;
                continue;
            }

            var quest = Managers.Quest.GetActiveQuest(questData);
            if (quest == null)
            {
                continue;
            }

            if (quest.State == QuestState.Completable)
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
