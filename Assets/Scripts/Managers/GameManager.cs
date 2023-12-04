using System;
using UnityEngine;

public class GameManager
{
    public event Action ResourceLoaded;
    public event Action GameStarted;

    public bool IsDefaultSpawnPosition { get; set; }
    public bool IsPortalSpawnPosition { get; set; }

    public void OnResourceLoaded()
    {
        ResourceLoaded?.Invoke();
        ResourceLoaded = null;
    }

    public void OnGameStarted()
    {
        GameStarted?.Invoke();
        GameStarted = null;
    }
}
