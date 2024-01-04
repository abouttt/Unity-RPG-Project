using UnityEngine;

public static class Extension
{
    public static T GetOrAddComponent<T>(this GameObject go) where T : Component
    {
        return Util.GetOrAddComponent<T>(go);
    }

    public static GameObject FindChild(this GameObject go, string name = null, bool recursive = false)
    {
        return Util.FindChild(go, name, recursive);
    }

    public static T FindChild<T>(this GameObject go, string name = null, bool recursive = false) where T : Object
    {
        return Util.FindChild<T>(go, name, recursive);
    }

    public static string GetLastSlashString(this string str)
    {
        return Util.GetLastSlashString(str);
    }

    public static Sprite ToSprite(this Texture2D texture)
    {
        return Util.Texture2DToSprite(texture);
    }
}
