using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDatabase : Singleton<ItemDatabase>
{
    private Dictionary<int, ItemData> m_itemCollection;

    public override void Awake()
    {
        base.Awake();

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

    public List<ItemData> GetItemsFromIDs(List<Tuple<int, int>> itemIDsQuantities)
    {
        int numItems = itemIDsQuantities.Count;
        List<ItemData> data = new List<ItemData>(numItems);

        for (int i = 0; i < numItems; i++)
        {
            ItemData d = GetItemFromIDWithQuantity(itemIDsQuantities[i]);
            if (d != null)
            {
                data.Add(d);
            }
        }

        return data;
    }

    public ItemData GetItemFromIDWithQuantity(Tuple<int, int> IdQuantity)
    {
        ItemData d = null;
        try
        {
            d = m_itemCollection[IdQuantity.m_First];
            d.m_Quantity = IdQuantity.m_Second;
        }
        catch (System.Exception e)
        {
            Debug.LogWarningFormat("<!> Unable to get ItemData with ID {0}", IdQuantity.m_First);
        }

        return d;
    }
}
