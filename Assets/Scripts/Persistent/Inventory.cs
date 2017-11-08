using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Inventory
{
    [SerializeField] private List<ItemData> m_Inventory;

    [SerializeField] private float m_Money;
    public float Money { get { return m_Money; } }

#if UNITY_EDITOR
    public int Count { get { return m_Inventory.Count; } }
    public int Capacity { get { return m_Inventory.Capacity; } }
    public string ItemNameAt(int i) { return m_Inventory[i].m_ItemName; }
#endif

    public Inventory(int maxSize)
    {
        m_Inventory = new List<ItemData>(maxSize);
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
    }
}
