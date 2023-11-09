using System.Collections;
using UnityEngine;

public class Managers : Singleton<Managers>
{
    public static PoolManager Pool => GetInstance._pool;

    private readonly PoolManager _pool = new();

    public static void Init()
    {
        Pool.Init();
    }

    public static void Clear()
    {
        Pool.Clear();
    }

    private void OnApplicationQuit()
    {
        Clear();
    }
}
