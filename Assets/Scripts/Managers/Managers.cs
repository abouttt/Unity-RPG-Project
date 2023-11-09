using System.Collections;
using UnityEngine;

public class Managers : Singleton<Managers>
{
    public static InputManager Input => GetInstance._input;
    public static PoolManager Pool => GetInstance._pool;
    public static ResourceManager Resource => GetInstance._resource;
    public static SceneManagerEx Scene => GetInstance._scene;
    public static SoundManager Sound => GetInstance._sound;
    public static UIManager UI => GetInstance._ui;

    private readonly InputManager _input = new();
    private readonly PoolManager _pool = new();
    private readonly ResourceManager _resource = new();
    private readonly SceneManagerEx _scene = new();
    private readonly SoundManager _sound = new();
    private readonly UIManager _ui = new();

    public static void Init()
    {
        Input.Init();
        Pool.Init();
        Sound.Init();
        UI.Init();
    }

    public static void Clear()
    {
        Input.Clear();
        Pool.Clear();
        Resource.Clear();
        Sound.Clear();
        UI.Clear();
    }

    private void OnApplicationQuit()
    {
        Clear();
    }
}
