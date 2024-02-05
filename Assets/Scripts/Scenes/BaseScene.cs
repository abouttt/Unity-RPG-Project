using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class BaseScene : MonoBehaviour
{
    [field: SerializeField]
    public SceneType SceneType { get; protected set; } = SceneType.Unknown;

    private int _currentLabelIndex = 0;

    private void Awake()
    {
        Managers.Init();
        InitDefaultPrefabs();
        InitResources(Init);
    }

    protected virtual void Init()
    {
        var eventSystem = FindObjectOfType(typeof(EventSystem));
        if (eventSystem == null)
        {
            Managers.Resource.Instantiate("EventSystem");
        }
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
