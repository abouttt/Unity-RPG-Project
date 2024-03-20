using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(menuName = "Database/Item Database", fileName = "ItemDatabase")]
public class ItemDatabase : SingletonScriptableObject<ItemDatabase>
{
    public IReadOnlyCollection<ItemData> Items => _items;

    [SerializeField]
    private List<ItemData> _items;

    public ItemData FindItemBy(string id)
    {
        return _items.FirstOrDefault(item => item.ItemID.Equals(id));
    }

#if UNITY_EDITOR
    [ContextMenu("Find Items")]
    public void FindItems()
    {
        FindItemsBy<ItemData>();
    }

    private void FindItemsBy<T>() where T : ItemData
    {
        _items = new();

        var guids = AssetDatabase.FindAssets($"t:{typeof(T)}");
        foreach (var guid in guids)
        {
            var assetPath = AssetDatabase.GUIDToAssetPath(guid);
            T item = AssetDatabase.LoadAssetAtPath<T>(assetPath);
            _items.Add(item);
        }

        EditorUtility.SetDirty(this);
        AssetDatabase.SaveAssets();
    }
#endif
}
