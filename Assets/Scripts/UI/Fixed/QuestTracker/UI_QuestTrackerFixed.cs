using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_QuestTrackerFixed : UI_Base
{
    enum Layouts
    {
        QuestTracker,
    }

    [SerializeField]
    private int _limitCount;

    private readonly Dictionary<Quest, UI_QuestTrackerSubitem> _trackers = new();

    protected override void Init()
    {
        Managers.UI.Register<UI_QuestTrackerFixed>(this);

        Bind<VerticalLayoutGroup>(typeof(Layouts));
    }

    public bool AddTracker(Quest quest)
    {
        if (_trackers.Count == _limitCount)
        {
            return false;
        }

        if (_trackers.TryGetValue(quest, out var _))
        {
            return false;
        }

        var layoutTransform = Get<VerticalLayoutGroup>((int)Layouts.QuestTracker).transform;
        var go = Managers.Resource.Instantiate("UI_QuestTrackerSubitem.prefab", layoutTransform, true);
        var tracker = go.GetComponent<UI_QuestTrackerSubitem>();
        tracker.SetQuest(quest);
        tracker.gameObject.SetActive(true);
        _trackers.Add(quest, tracker);
        LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)layoutTransform);

        return true;
    }

    public void RemoveTracker(Quest quest)
    {
        if (_trackers.TryGetValue(quest, out var tracker))
        {
            tracker.Clear();
            _trackers.Remove(quest);
            Managers.Resource.Destroy(tracker.gameObject);
        }
    }
}
