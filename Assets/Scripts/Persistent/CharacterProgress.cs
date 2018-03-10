using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CharacterProgress
{
    // TODO
    // Store the character stats here
    // Store the inventory here

    public Inventory m_Inventory;
    public CharacterStats m_Stats;

    private int m_MaxInventorySize = 20;

    // this constructor somehow gets called with no callstack as soon as the game launches
    // could be a secret built in unity class?

    //public CharacterProgress()
    //{
    //    m_Inventory = new Inventory(m_MaxInventorySize);
    //    m_Stats = new CharacterStats();
    //}

    // called after a successful load
    public void Init()
    {
        m_Inventory = new Inventory(m_MaxInventorySize);
        m_Stats = new CharacterStats();
    }

    public void Save()
    {
        m_Inventory.SetIDsFromInventory();
    }

    public void Load()
    {
        m_Inventory.BuildInventoryFromIDs();
    }
}
