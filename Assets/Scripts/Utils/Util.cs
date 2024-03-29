using System.Linq;
using UnityEngine;

public static class Util
{
    public static T GetOrAddComponent<T>(GameObject go) where T : Component
    {
        if (!go.TryGetComponent(out T component))
        {
            component = go.AddComponent<T>();
        }

        return component;
    }

    public static GameObject FindChild(GameObject go, string name = null, bool recursive = false)
    {
        var transform = FindChild<Transform>(go, name, recursive);
        return transform == null ? null : transform.gameObject;
    }

    public static T FindChild<T>(GameObject go, string name = null, bool recursive = false) where T : Object
    {
        if (go == null)
        {
            return null;
        }

        if (recursive)
        {
            return go.GetComponentsInChildren<T>().FirstOrDefault(component => string.IsNullOrEmpty(name) || component.name.Equals(name));
        }
        else
        {
            for (int i = 0; i < go.transform.childCount; i++)
            {
                var transform = go.transform.GetChild(i);
                if (string.IsNullOrEmpty(name) || transform.name.Equals(name))
                {
                    if (transform.TryGetComponent<T>(out var component))
                    {
                        return component;
                    }
                }
            }
        }

        return null;
    }

    public static GameObject FindOrInstantiate(string name)
    {
        var go = GameObject.Find(name);
        if (go == null)
        {
            go = new GameObject { name = name };
        }

        return go;
    }

    public static string GetLastSlashString(string str)
    {
        string result = str;
        int index = str.LastIndexOf('/');
        if (index >= 0)
        {
            result = str[(index + 1)..];
        }

        return result;
    }

    public static Sprite Texture2DToSprite(Texture2D texture)
    {
        var rect = new Rect(0, 0, texture.width, texture.height);
        return Sprite.Create(texture, rect, new Vector2(0.5f, 0.5f));
    }

    public static int CalcDamage(int damage, int defense)
    {
        return (int)(damage - (defense * 0.5f));
    }

    public static int CalcIncreasePer(int value, int per)
    {
        return (int)(value * (1f + per / 100f));
    }
}
