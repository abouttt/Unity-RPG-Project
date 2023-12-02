using UnityEngine;

public class Portal : Interactive
{
    [SerializeField]
    private SceneType _loadScene;

    public override void Interaction()
    {
        Managers.Game.IsPortalSpawnPosition = true;
        Managers.Scene.LoadScene(_loadScene);
    }
}
