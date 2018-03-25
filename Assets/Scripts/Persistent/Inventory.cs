using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO remove this and rewrite the stuff that uses it

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
    [SerializeField] private List<InventoryItem> m_Inventory;

    [SerializeField] private float m_Money;
    public float Money { get { return m_Money; } }

#if UNITY_EDITOR
    // for debug display purposes
    public int Count { get { return m_Inventory.Count; } }
    public int Capacity { get { return m_Inventory.Capacity; } }
    public string ItemNameAt(int i) { return m_Inventory[i].Name; }
    public int QuantityAt(int i) { return m_Inventory[i].Quantity; }
#endif

    public Inventory(int maxSize)
    {
        m_Inventory = new List<InventoryItem>(maxSize);
    }

    public bool Add(InventoryItem item)
    {
        if (item.Type == Enums.eItemType.Money)
        {
            m_Money += item.Value;
            return true;
        }
        else
        {
            InventoryItem i = FindItemWithID(item.ID);
            if (i != null)
            {
                // Item already existed, increment quantity
                i.Quantity += item.Quantity;
                return true;
            }
            else
            {
                // Item didn't exist, add it
                if (m_Inventory.Count < m_Inventory.Capacity)
                {
                    m_Inventory.Add(item);
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

    public bool IsItemUsable(int i)
    {
        bool hasUsable = ((IUsable)m_Inventory[i] != null);
        return hasUsable;
    }

    public void UseItem(InventoryItem i)
    {
        IUsable item = (IUsable)i;
        PlayableCharacter player = (PlayableCharacter)CharacterManager.Instance.PlayerCharacter;

        if (player != null)
        {
            item.Use(player);
        }
        else
        {
            Debug.LogErrorFormat("Unable to get Playable Character");
        }
    }

    public InventoryItem GetItemAt(int i)
    {
        return m_Inventory[i];
    }

    private InventoryItem FindItemWithID(int id)
    {
        InventoryItem item = null;
        for (int i = 0; i < m_Inventory.Count; i++)
        {
            if (m_Inventory[i].ID == id)
            {
                item = m_Inventory[i];
                break;
            }
        }

        return item;
    }

    public void Remove(InventoryItem i)
    {
        InventoryItem item = FindItemWithID(i.ID);
        if (item != null)
        {
            if (item.Quantity > 1)
            {
                item.Quantity -= i.Quantity;
            }
            else
            {
                m_Inventory.Remove(item);
            }
        }
        else
        {
            Debug.LogWarningFormat("Tried to remove ItemData with ID {0} when it wasn't in the inventory", i.ID);
        }
    }

    //public void SetIDsFromInventory()
    //{
    //    m_ItemIDQuantityPairs.Clear();

    //    for (int i = 0; i < m_Inventory.Count; i++)
    //    {
    //        ItemData d = m_Inventory[i];
    //        m_ItemIDQuantityPairs.Add(new Tuple<int, int>(d.ItemID, d.m_Quantity));
    //    }
    //}

    //public void BuildInventoryFromIDs()
    //{
    //    List<ItemData> data = ItemDatabase.Instance.GetItemsFromIDs(m_ItemIDQuantityPairs);
    //    m_Inventory = new List<ItemData>(m_Capacity);
    //    for (int i = 0; i < data.Count; i++)
    //    {
    //        m_Inventory.Add(data[i]);
    //    }

    //    m_ItemIDQuantityPairs.Clear();
    //}
}
