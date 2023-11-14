using System;
using UnityEngine;

public class Test : MonoBehaviour
{
    [Serializable]
    public class ItemDataTest
    {
        public ItemData ItemData;
        public int Count;
    }

    public ItemDataTest[] ItemDatas;

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.B))
        {
            foreach (var itemData in ItemDatas)
            {
                Player.ItemInventory.AddItem(itemData.ItemData, itemData.Count);
            }
        }
    }
}
