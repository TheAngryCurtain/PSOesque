using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    [SerializeField] private ItemData m_ItemData;
    public ItemData Data { get { return m_ItemData; } }

    public void SetData(ItemData data)
    {
        m_ItemData = data;
    }
}
