using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Inventory
{
    [System.NonSerialized] private List<ItemData> m_Inventory;

    // this is a work-around for not being able to save the ItemData scriptableObjects
    // just store the list of IDs and save them out instead
    [SerializeField] private List<int> m_InventoryIds;
    [SerializeField] private int m_Capacity;

    [SerializeField] private float m_Money;
    public float Money { get { return m_Money; } }

#if UNITY_EDITOR
    public int Count { get { return m_Inventory.Count; } }
    public int Capacity { get { return m_Inventory.Capacity; } }
    public string ItemNameAt(int i) { return m_Inventory[i].m_ItemName; }
#endif

    public Inventory(int maxSize)
    {
        m_Capacity = maxSize;

        m_Inventory = new List<ItemData>(maxSize);
        m_InventoryIds = new List<int>(maxSize);
    }

    public bool Add(ItemData d)
    {
        if (d.m_ItemType == Enums.eItemType.Money)
        {
            m_Money += d.m_ItemValue;
            return true;
        }
        else
        {
            if (m_Inventory.Count < m_Inventory.Capacity)
            {
                m_Inventory.Add(d);
                m_InventoryIds.Add(d.ItemID);
                return true;
            }
            else
            {
                Debug.LogWarningFormat("Inventory is full! count: {0}, capacity: {1}", m_Inventory.Count, m_Inventory.Capacity);
                return false;
            }
        }
    }

    public void Remove(ItemData d)
    {
        m_Inventory.Remove(d);
        m_InventoryIds.Remove(d.ItemID);
    }

    public void PopulateOnLoad()
    {
        List<ItemData> data = ItemDatabase.Instance.GetItemsFromIDs(m_InventoryIds);
        m_Inventory = new List<ItemData>(m_Capacity);
        for (int i = 0; i < data.Count; i++)
        {
            m_Inventory.Add(data[i]);
        }
    }
}
