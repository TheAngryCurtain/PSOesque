using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CharacterProgress
{
    public Inventory m_Inventory;
    public CharacterStats m_Stats;

    private int m_MaxInventorySize = 20;

    public CharacterProgress()
    {
    
    }

    public void Init()
    {
        m_Inventory = new Inventory(m_MaxInventorySize);

        m_Stats = new CharacterStats();
        m_Stats.Init();
    }
}
