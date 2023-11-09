using System.Collections;
using UnityEngine;

public class Managers : Singleton<Managers>
{
    public static InputManager Input => GetInstance._input;
    public static PoolManager Pool => GetInstance._pool;

    private readonly InputManager _input = new();
    private readonly PoolManager _pool = new();

    public static void Init()
    {
        Input.Init();
        Pool.Init();
    }

    public static void Clear()
    {
        Input.Clear();
        Pool.Clear();
    }

    private void OnApplicationQuit()
    {
        Clear();
    }
}
