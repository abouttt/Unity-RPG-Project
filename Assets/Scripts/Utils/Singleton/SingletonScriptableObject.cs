using UnityEngine;

public class SingletonScriptableObject<T> : ScriptableObject where T : SingletonScriptableObject<T>
{
    private static T s_instance;

    public static T GetInstance
    {
        get
        {
            if (s_instance == null)
            {
                T[] asset = Resources.LoadAll<T>("");

                if (asset == null || asset.Length == 0)
                {
                    throw new System.Exception($"Could not find any singleton scriptable object instance in the resources.");
                }
                else if (asset.Length > 1)
                {
                    Debug.Log("Multiple instance of the singleton scriptable object found in the resources.");
                }

                s_instance = asset[0];
            }

            return s_instance;
        }
    }
}
