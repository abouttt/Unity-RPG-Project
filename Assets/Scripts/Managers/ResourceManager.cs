using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Object = UnityEngine.Object;

public class ResourceManager
{
    public bool HasResources => _resources.Count > 0;

    private readonly Dictionary<string, Object> _resources = new();

    public T Load<T>(string key) where T : Object
    {
        if (_resources.TryGetValue(key, out var resource))
        {
            return resource as T;
        }

        return null;
    }

    public GameObject Instantiate(string key, Transform parent = null, bool pooling = false)
    {
        var prefab = Load<GameObject>(key);
        if (prefab == null)
        {
            Debug.Log($"[ResourceManager/Instantiate] Faild to load prefab : {key}");
            return null;
        }

        if (pooling)
        {
            return Managers.Pool.Pop(prefab, parent);
        }

        var go = Object.Instantiate(prefab, parent);
        go.name = prefab.name;
        return go;
    }

    public GameObject Instantiate(string key, Vector3 position, Transform parent = null, bool pooling = false)
    {
        var go = Instantiate(key, parent, pooling);

        if (parent == null)
        {
            go.transform.position = position;
        }
        else
        {
            go.transform.localPosition = position;
        }

        return go;
    }

    public GameObject Instantiate(string key, Vector3 position, Quaternion rotation, Transform parent = null, bool pooling = false)
    {
        var go = Instantiate(key, parent, pooling);

        if (parent == null)
        {
            go.transform.SetPositionAndRotation(position, rotation);
        }
        else
        {
            go.transform.SetLocalPositionAndRotation(position, rotation);
        }

        return go;
    }

    public void Destroy(GameObject go)
    {
        if (go == null)
        {
            return;
        }

        if (Managers.Pool.Push(go))
        {
            return;
        }

        Object.Destroy(go);
    }

    public void LoadAsync<T>(string key, Action<T> callback = null) where T : Object
    {
        if (_resources.TryGetValue(key, out var resource))
        {
            callback?.Invoke(resource as T);
        }
        else
        {
            Addressables.LoadAssetAsync<T>(key).Completed += op =>
            {
                if (!_resources.ContainsKey(key))
                {
                    _resources.Add(key, op.Result);
                }

                callback?.Invoke(op.Result);
            };
        }
    }

    public void LoadAllAsync<T>(string label, Action callback) where T : Object
    {
        Addressables.LoadResourceLocationsAsync(label, typeof(T)).Completed += op =>
        {
            int loadCount = 0;
            int totalCount = op.Result.Count;

            if (totalCount == 0)
            {
                callback?.Invoke();
            }
            else
            {
                foreach (var result in op.Result)
                {
                    LoadAsync<T>(result.PrimaryKey, obj =>
                    {
                        loadCount++;
                        if (loadCount == totalCount)
                        {
                            callback?.Invoke();
                        }
                    });
                }
            }
        };
    }

    public void Clear()
    {
        foreach (var resource in _resources)
        {
            Addressables.Release(resource.Value);
        }

        _resources.Clear();
    }
}
