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

    public ItemData FindItemBy(string id) => _items.FirstOrDefault(q => q.ItemID.Equals(id));

#if UNITY_EDITOR
    [ContextMenu("Find Items")]
    private void FindItems()
    {
        FindItemsBy<ItemData>();
    }

    private void FindItemsBy<T>() where T : ItemData
    {
        _items = new List<ItemData>();
        string[] guids = AssetDatabase.FindAssets($"t:{typeof(T)}");
        foreach (string guid in guids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            var item = AssetDatabase.LoadAssetAtPath<T>(assetPath);

            _items.Add(item);

            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
        }
    }
#endif
}
