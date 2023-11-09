using System.Collections;
using UnityEngine;

public class Managers : Singleton<Managers>
{
    public static InputManager Input => GetInstance._input;
    public static PoolManager Pool => GetInstance._pool;
    public static ResourceManager Resource => GetInstance._resource;
    public static UIManager UI => GetInstance._ui;

    private readonly InputManager _input = new();
    private readonly PoolManager _pool = new();
    private readonly ResourceManager _resource = new();
    private readonly UIManager _ui = new();

    public static void Init()
    {
        Input.Init();
        Pool.Init();
        UI.Init();
    }

    public static void Clear()
    {
        Input.Clear();
        Pool.Clear();
        Resource.Clear();
        UI.Clear();
    }

    private void OnApplicationQuit()
    {
        Clear();
    }
}
