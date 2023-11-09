using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : Component
{
    private static T s_instance;

    public static T GetInstance
    {
        get
        {
            if (s_instance == null)
            {
                s_instance = FindObjectOfType<T>();
                if (s_instance == null)
                {
                    var go = new GameObject { name = $"{typeof(T).Name}" };
                    s_instance = go.AddComponent<T>();
                }
            }

            return s_instance;
        }
    }

    protected virtual void Awake()
    {
        if (s_instance == null)
        {
            s_instance = this as T;
        }
        else if (s_instance != this)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
    }
}
