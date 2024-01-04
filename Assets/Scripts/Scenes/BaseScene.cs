using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class BaseScene : MonoBehaviour
{
    [field: SerializeField]
    public SceneType SceneType { get; protected set; } = SceneType.Unknown;
    [SerializeField]
    private AudioClip _sceneBGM;
    [SerializeField]
    private GameObject[] _prefabs;

    private int _currentLabelIndex = 0;

    private void Awake()
    {
        Managers.Init();
        InitDefaultPrefabs();
        InitResources(Init);
    }

    protected virtual void Init()
    {
        Managers.Game.OnResourceLoaded();
        Managers.Sound.Play(_sceneBGM, SoundType.Bgm);

        var eventSystem = FindObjectOfType(typeof(EventSystem));
        if (eventSystem == null)
        {
            Managers.Resource.Instantiate("EventSystem");
        }

        InitPrefabs();
    }

    private void InitDefaultPrefabs()
    {
        foreach (var prefab in SceneSetting.GetInstance.FirstCreatePrefabs)
        {
            Instantiate(prefab);
        }
    }

    private void InitPrefabs()
    {
        if (_prefabs != null)
        {
            foreach (var prefab in _prefabs)
            {
                Instantiate(prefab);
            }
        }
    }

    private void InitResources(Action callback)
    {
        var loadResourceLabels = SceneSetting.GetInstance.LoadResourceLabels[SceneType];

        if (loadResourceLabels == null || loadResourceLabels.Length == 0)
        {
            return;
        }

        Managers.Resource.LoadAllAsync<UnityEngine.Object>(loadResourceLabels[_currentLabelIndex].ToString(), () =>
        {
            if (_currentLabelIndex == loadResourceLabels.Length - 1)
            {
                callback?.Invoke();
            }
            else
            {
                _currentLabelIndex++;
                InitResources(callback);
            }
        });
    }
}
