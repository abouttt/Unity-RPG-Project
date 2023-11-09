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
        InitDefaultPrefab();
        InitResources(Init);
    }

    protected virtual void Init()
    {
        Managers.Sound.Play(_sceneBGM, SoundType.Bgm);

        var eventSystem = FindObjectOfType(typeof(EventSystem));
        if (eventSystem == null)
        {
            Managers.Resource.Instantiate("EventSystem");
        }

        InitPrefab();
    }

    private void InitDefaultPrefab()
    {
        foreach (var prefab in LoadSetting.GetInstance.DefaultPrefabs)
        {
            Instantiate(prefab);
        }
    }

    private void InitPrefab()
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
        var loadResourceLabels = LoadSetting.GetInstance.LoadResourceLabels[SceneType];

        if (loadResourceLabels == null || loadResourceLabels.Length == 0)
        {
            return;
        }

        Managers.Resource.LoadAllAsync<GameObject>(loadResourceLabels[_currentLabelIndex].ToString(), () =>
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
