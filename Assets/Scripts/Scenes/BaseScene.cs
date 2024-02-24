using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class BaseScene : MonoBehaviour
{
    [field: SerializeField]
    public SceneType SceneType { get; protected set; } = SceneType.Unknown;
    [SerializeField]
    private AudioClip _sceneBGM;

    private int _currentLabelIndex = 0;

    private void Awake()
    {
        Managers.Init();
        InitDefaultPrefabs();
        Init();
        Managers.Sound.Play(_sceneBGM, SoundType.Bgm);
    }

    protected virtual void Init()
    {
        var eventSystem = FindObjectOfType(typeof(EventSystem));
        if (eventSystem == null)
        {
            Managers.Resource.Instantiate("EventSystem");
        }
    }

    protected void LoadResourcesAsync(SceneType sceneType, Action callback = null)
    {
        var loadResourceLabels = SceneSetting.GetInstance.LoadResourceLabels[sceneType];

        if (loadResourceLabels == null || loadResourceLabels.Length == 0)
        {
            return;
        }

        Managers.Resource.LoadAllAsync<UnityEngine.Object>(loadResourceLabels[_currentLabelIndex].ToString(), () =>
        {
            if (_currentLabelIndex == loadResourceLabels.Length - 1)
            {
                _currentLabelIndex = 0;
                callback?.Invoke();
            }
            else
            {
                _currentLabelIndex++;
                LoadResourcesAsync(sceneType, callback);
            }
        });
    }

    protected void InitUIPackage(string packageName)
    {
        var UIPackage = Managers.Resource.Instantiate(packageName);
        UIPackage.transform.DetachChildren();
        Destroy(UIPackage);
    }

    private void InitDefaultPrefabs()
    {
        foreach (var prefab in SceneSetting.GetInstance.DefaultPrefabs)
        {
            Instantiate(prefab);
        }
    }

    private void OnDestroy()
    {
        Managers.Clear();
    }
}
