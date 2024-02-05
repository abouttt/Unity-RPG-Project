using System;
using UnityEngine;

public class GameManager
{
    public event Action GameStarted;

    public bool IsDefaultSpawnPosition { get; set; }
    public bool IsPortalSpawnPosition { get; set; }

    public void OnGameStarted()
    {
        GameStarted?.Invoke();
        GameStarted = null;
    }
}
