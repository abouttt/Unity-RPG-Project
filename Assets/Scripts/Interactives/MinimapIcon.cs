using UnityEngine;

public class MinimapIcon : MonoBehaviour
{
    public string IconName { get; private set; }

    public void Setup(string spriteName, string iconName)
    {
        GetComponent<SpriteRenderer>().sprite = Managers.Resource.Load<Texture2D>(spriteName).ToSprite();
        IconName = iconName;
    }
}
