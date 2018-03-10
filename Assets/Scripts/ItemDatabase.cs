using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDatabase : Singleton<ItemDatabase>
{
    private Dictionary<int, ItemData> m_itemCollection;

    private void Awake()
    {
        LoadItems();
    }

    private void LoadItems()
    {
        m_itemCollection = new Dictionary<int, ItemData>();
        ItemData[] loaded = Resources.LoadAll<ItemData>("Items/Data");

        int totalLoaded = loaded.Length;
        for (int i = 0; i < totalLoaded; i++)
        {
            ItemData data = loaded[i];
            m_itemCollection.Add(data.ItemID, data);
        }
    }

    public List<ItemData> GetItemsFromIDs(List<int> itemIDs)
    {
        int numItems = itemIDs.Count;
        List<ItemData> data = new List<ItemData>(numItems);

        for (int i = 0; i < numItems; i++)
        {
            int id = itemIDs[i];
            data.Add(m_itemCollection[id]);
        }

        return data;
    }
}
