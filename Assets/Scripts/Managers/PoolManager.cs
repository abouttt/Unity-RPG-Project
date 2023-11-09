using System.Collections.Generic;
using UnityEngine;

public class PoolManager
{
    #region Pool
    private class Pool
    {
        public GameObject Original { get; private set; }
        public Transform Root { get; private set; }

        private readonly Stack<GameObject> _pool = new();

        public void Init(GameObject original, int count)
        {
            Original = original;
            Root = new GameObject($"{Original.name}_Root").transform;

            for (int i = 0; i < count; i++)
            {
                Push(Create());
            }
        }

        public void Push(GameObject go)
        {
            go.transform.SetParent(Root);
            go.SetActive(false);
            _pool.Push(go);
        }

        public GameObject Pop(Transform parent)
        {
            var go = _pool.Count > 0 ? _pool.Pop() : Create();
            go.SetActive(true);
            go.transform.SetParent((parent == null) ? Root : parent);
            return go;
        }

        private GameObject Create()
        {
            var go = Object.Instantiate(Original);
            go.name = Original.name;
            return go;
        }
    }
    #endregion

    private readonly Dictionary<string, Pool> _pools = new();
    private Transform _root;

    public void Init()
    {
        _root = Util.FindOrInstantiate("Pool_Root").transform;
    }

    public void CreatePool(GameObject original, int count = 3)
    {
        if (original == null)
        {
            return;
        }

        var name = original.name;
        if (_pools.ContainsKey(name))
        {
            Debug.Log($"[PoolManager/CreatePool] The {name}_Pool already exist.");
            return;
        }

        var pool = new Pool();
        pool.Init(original, count);
        pool.Root.SetParent(_root);

        _pools.Add(name, pool);
    }

    public bool Push(GameObject go)
    {
        if (go == null)
        {
            return false;
        }

        if (!_pools.ContainsKey(go.name))
        {
            return false;
        }

        _pools[go.name].Push(go);

        return true;
    }

    public GameObject Pop(GameObject original, Transform parent = null)
    {
        var name = original.name;
        if (!_pools.ContainsKey(name))
        {
            CreatePool(original);
        }

        return _pools[name].Pop(parent);
    }

    public GameObject GetOriginal(string name)
    {
        if (!_pools.ContainsKey(name))
        {
            return null;
        }

        return _pools[name].Original;
    }

    public void ClearPool(string name)
    {
        if (!_pools.ContainsKey(name))
        {
            Debug.Log($"[PoolManager/ClearPool] The {name}_Pool no exist.");
            return;
        }

        Object.Destroy(_pools[name].Root.gameObject);
        _pools.Remove(name);
    }

    public void Clear()
    {
        if (_root != null)
        {
            foreach (Transform child in _root)
            {
                Object.Destroy(child.gameObject);
            }
        }

        _pools.Clear();
    }
}
