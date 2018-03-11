using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDatabase : Singleton<ItemDatabase>
{
    private Dictionary<int, ItemData> m_itemCollection;
    private string itemDataXMLPath;

    public override void Awake()
    {
        itemDataXMLPath = Application.streamingAssetsPath + "/XML/ItemDatabase.xml";

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

    public List<InventoryItem> GetItemsFromIDs(List<Tuple<int, int>> itemIDsQuantities)
    {
        int numItems = itemIDsQuantities.Count;
        List<InventoryItem> items = new List<InventoryItem>(numItems);

        for (int i = 0; i < numItems; i++)
        {
            InventoryItem d = GetItemFromIDWithQuantity(itemIDsQuantities[i]);
            if (d != null)
            {
                items.Add(d);
            }
        }

        return items;
    }
    
    public List<Enums.eLevelTheme> GetThemesForItem(int id)
    {
        ItemData d = null;
        try
        {
            d = m_itemCollection[id];
        }
        catch (System.Exception e)
        {
            Debug.LogWarningFormat("<!> Unable to get ItemData with ID {0}", id);
        }

        return d.m_Themes;
    }

    public List<Enums.eDifficulty> GetDifficultiesForItem(int id)
    {
        ItemData d = null;
        try
        {
            d = m_itemCollection[id];
        }
        catch (System.Exception e)
        {
            Debug.LogWarningFormat("<!> Unable to get ItemData with ID {0}", id);
        }

        return d.m_Difficulties;
    }

    public InventoryItem GetItemFromID(int id)
    {
        return GetItemFromIDWithQuantity(new Tuple<int, int>(id, 1));
    }

    public InventoryItem GetItemFromIDWithQuantity(Tuple<int, int> IdQuantity)
    {
        ItemData d = null;
        try
        {
            d = m_itemCollection[IdQuantity.m_First];
        }
        catch (System.Exception e)
        {
            Debug.LogWarningFormat("<!> Unable to get ItemData with ID {0}", IdQuantity.m_First);
        }

        // build item
        InventoryItem item = new InventoryItem(d.ItemID);
        item.Name = d.m_ItemName;
        item.Description = d.m_ItemDescription;
        item.Value = d.m_ItemValue;
        item.Quantity = IdQuantity.m_Second;
        item.Type = d.m_ItemType;

        return item;
    }
}
