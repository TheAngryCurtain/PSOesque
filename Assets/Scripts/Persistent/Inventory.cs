using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Tuple<T1, T2>
{
    public T1 m_First;
    public T2 m_Second;

    public Tuple(T1 first, T2 second)
    {
        m_First = first;
        m_Second = second;
    }
}

[System.Serializable]
public class Inventory
{
    [System.NonSerialized] private List<ItemData> m_Inventory;

    // this is a work-around for not being able to save the ItemData scriptableObjects
    // this list is built just before a save, and is considered out of date until the next save.
    [SerializeField] private List<Tuple<int, int>> m_ItemIDQuantityPairs;
    [SerializeField] private int m_Capacity;

    [SerializeField] private float m_Money;
    public float Money { get { return m_Money; } }

#if UNITY_EDITOR
    // for debug display purposes
    public int Count { get { return m_Inventory.Count; } }
    public int Capacity { get { return m_Inventory.Capacity; } }
    public string ItemNameAt(int i) { return m_Inventory[i].m_ItemName; }
    public int QuantityAt(int i) { return m_Inventory[i].m_Quantity; }
#endif

    public Inventory(int maxSize)
    {
        m_Capacity = maxSize;

        m_Inventory = new List<ItemData>(maxSize);
        m_ItemIDQuantityPairs = new List<Tuple<int, int>>(maxSize);
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
            ItemData item = FindItemWithID(d.ItemID);
            if (item != null)
            {
                // Item already existed, increment quantity
                item.m_Quantity += d.m_Quantity;
                return true;
            }
            else
            {
                // Item didn't exist, add it
                if (m_Inventory.Count < m_Inventory.Capacity)
                {
                    m_Inventory.Add(d);
                    return true;
                }
                else
                {
                    Debug.LogWarningFormat("Inventory is full! count: {0}, capacity: {1}", m_Inventory.Count, m_Inventory.Capacity);
                    return false;
                }
            }
        }
    }

    private ItemData FindItemWithID(int id)
    {
        ItemData d = null;
        for (int i = 0; i < m_Inventory.Count; i++)
        {
            if (m_Inventory[i].ItemID == id)
            {
                d = m_Inventory[i];
                break;
            }
        }

        return d;
    }

    public void Remove(ItemData d)
    {
        ItemData item = FindItemWithID(d.ItemID);
        if (item != null)
        {
            if (item.m_Quantity > 1)
            {
                item.m_Quantity -= d.m_Quantity;
            }
            else
            {
                m_Inventory.Remove(item);
            }
        }
        else
        {
            Debug.LogWarningFormat("Tried to remove ItemData with ID {0} when it wasn't in the inventory", d.ItemID);
        }
    }

    public void SetIDsFromInventory()
    {
        m_ItemIDQuantityPairs.Clear();

        for (int i = 0; i < m_Inventory.Count; i++)
        {
            ItemData d = m_Inventory[i];
            m_ItemIDQuantityPairs.Add(new Tuple<int, int>(d.ItemID, d.m_Quantity));
        }
    }

    public void BuildInventoryFromIDs()
    {
        List<ItemData> data = ItemDatabase.Instance.GetItemsFromIDs(m_ItemIDQuantityPairs);
        m_Inventory = new List<ItemData>(m_Capacity);
        for (int i = 0; i < data.Count; i++)
        {
            m_Inventory.Add(data[i]);
        }

        m_ItemIDQuantityPairs.Clear();
    }
}
